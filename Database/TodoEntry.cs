using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace websLINE.Database
{
    public class TodoEntry : IDbObject
    {
        private const string fileName = "todo_entries.json";
        private static readonly object @lock = new object();
        private const int attempts = 3;
        /// <summary>
        /// 3 sec delay if the file is locked
        /// </summary>
        private const int delay = 3000;
        public TodoEntry()
        {

        }
        #region properties
        public int Id { get; set; }
        /// <summary>
        /// ToDo add purpose
        /// </summary>
        public bool Deleted { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? FinishedAt { get; set; } = null;
        #endregion
        #region public methods
        public static List<TodoEntry> Get()
        {
            List<TodoEntry> todoEntries = DeSerializeToDoEntry();
            if (todoEntries != null && todoEntries.Count > 0)
            {
                todoEntries = todoEntries.OrderBy(x => x.Id).ToList();
            }
            else
            {
                //Load standard values
                todoEntries = GetStandard();
            }
            return todoEntries;
        }

        public static TodoEntry Get(int id)
        {
            List<TodoEntry> todoEntries = DeSerializeToDoEntry();
            //Receive the value
            return Get().FirstOrDefault(x => x.Id == id);
        }

        public static void SerializeToDoEntry(List<TodoEntry> todoEntries)
        {
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            // Serialize to JSON
            string json = JsonConvert.SerializeObject(todoEntries, Newtonsoft.Json.Formatting.Indented);
            // Write to file
            WriteToFile(filePath, json);
        }
        #endregion
        private static void WriteToFile(string filePath, string content)
        {
            int retryAttempts = 0;
            bool success = false;

            while (!success && retryAttempts < attempts)
            {
                try
                {
                    lock (@lock)
                    {
                        File.WriteAllText(filePath, content);
                    }
                    success = true; // File write succeeded
                }
                catch (Exception ex)
                {
                    //ToDo add HRESULT Error Codes
                    //Retry after delay
                    retryAttempts++;
                    Console.WriteLine($"File {filePath} is locked.");
                    Thread.Sleep(delay); // Wait before retrying
                }
            }

            if (!success)
            {
                throw new Exception($"Failed to write to File {filePath}");
            }
        }
        private static List<TodoEntry> DeSerializeToDoEntry()
        {
            List<TodoEntry> toDoEntries = null;
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            if (System.IO.File.Exists(filePath))
            {
                // Read JSON from file
                string json = File.ReadAllText(filePath);
                // Deserialize JSON to list of TodoEntry objects
                toDoEntries = JsonConvert.DeserializeObject<List<TodoEntry>>(json);
            }
            return toDoEntries;
        }
        private static List<TodoEntry> GetStandard()
        {
            List<TodoEntry> returnValue = new List<TodoEntry>();
            //Add standard values
            returnValue.Add(new TodoEntry()
            {
                Id = 1,
                Caption = "Aufgabe 1",
                Description = "Diese Tabelle sollte über eine Refresh-Funktion verfügen, welche durch einen Button (oben rechts) ausgeführt werden kann. Die Daten sollten mittels Ajax geladen werden."
            });

            returnValue.Add(new TodoEntry()
            {
                Id = 2,
                Caption = "Aufgabe 2",
                Description = "Diese Tabelle sollte über einen Hinzufügen-Button verfügen, welcher eine leere Aufgabe (Caption = Aufgabe 3) hinzufügt. Anschließend sollte die Refresh-Funktion aufgerufen werden."
            });

            returnValue.Add(new TodoEntry()
            {
                Id = 4,
                Caption = "Aufgabe 4",
                Description = "Diese Tabelle sollte anhand des Id-Feldes korrekt sortiert dargestellt werden."
            });

            returnValue.Add(new TodoEntry()
            {
                Id = 5,
                Caption = "Aufgabe 5",
                Description = "Füge eine weitere Spalte zur Tabelle hinzu, welche über eine Checkbox verfügt, um einzelne Aufgaben als erledigt markieren zu können. Bei jedem klick sollte ein Ajax Aufruf stattfinden, welcher das aktuelle Datum / Uhrzeit im Feld 'FinishedAt' festhält."
            });

            returnValue.Add(new TodoEntry()
            {
                Id = 6,
                Caption = "Aufgabe 6",
                Description = "Sorge dafür, dass die Daten persistiert werden sobald Änderungen daran stattfinden - Die Daten kannst Du entweder in einer SQLlite Datenbank oder einer JSON Datei speichern - Die Daten sollten zukünftig von dieser Datenquelle geladen werden."
            });

            returnValue.Add(new TodoEntry()
            {
                Id = 7,
                Caption = "Aufgabe 7",
                Description = "Wenn alle Aufgaben als erledigt markiert wurden, sollte ein grüner Block unterhalb der Tabelle erscheinen welcher den Text 'Alle Aufgaben wurden erfolgreich erledigt' beinhaltet."
            });
            //Create the file
            SerializeToDoEntry(returnValue);
            return returnValue;
        }
    }
}
