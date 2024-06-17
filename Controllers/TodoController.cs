using Microsoft.AspNetCore.Mvc;
using websLINE.Database;
using websLINE.Helper;

namespace websLINE.Controllers
{

    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AjaxTable()
        {
            //Load the data initially
            List<TodoEntry> entries = MockingBase.TodoEntries.Select(e => e.Deleted == false);
            return View(entries);
        }

        [HttpGet("GetTodoEntries")]
        public IActionResult GetTodoEntries()
        {
            List<TodoEntry> entries = TodoEntry.Get();
            if (entries != null)
            {
                return Ok(entries);
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult NewOrEdit(int? id)
        {
            TodoEntry entry = new TodoEntry();
            if (id != null)
            {
                entry = MockingBase.TodoEntries.Get(id.Value) ?? new TodoEntry();
            }

            return View(entry);
        }

        [HttpPost]
        public IActionResult SaveEntry(int id)
        {
            TodoEntry entry;
            if (id != 0)
            {               
                entry = TodoEntry.Get(id) ?? new TodoEntry();
                //Load values
                string caption = Request.Form.GetString("Caption");
                string description = Request.Form.GetString("Description");
                string finishedString = Request.Form.GetString("Finished");
                bool finished = !string.IsNullOrEmpty(finishedString) ? bool.Parse(finishedString) : false;
                //Set values
                entry.Caption = !string.IsNullOrEmpty(caption) ? caption : entry.Caption;
                entry.Description = !string.IsNullOrEmpty(description) ? description : entry.Description;
                if (finished == false)
                {
                    entry.FinishedAt = null;
                }
                else
                {
                    entry.FinishedAt = entry.FinishedAt is null ? DateTime.Now : entry.FinishedAt;
                }
                Update(entry);
            }
            else
            {
                entry = new TodoEntry();
                entry.FinishedAt = null;
                AddNew(entry);
            }
            return Ok();
        }


        private static void Update(TodoEntry toDoEntry)
        {
            //Receive stored values
            List<TodoEntry> toDoEntries = TodoEntry.Get();
            TodoEntry existingToDoEntry = toDoEntries.FirstOrDefault(x => x.Id == toDoEntry.Id);
            if (existingToDoEntry != null)
            {
                int index = toDoEntries.IndexOf(existingToDoEntry);
                toDoEntries[index] = toDoEntry;
                TodoEntry.SerializeToDoEntry(toDoEntries);
            }
            else
            {
                AddNew(toDoEntry);
            }
        }


        private static int GetLowestID(List<int> numbers)
        {
            //Sort the list
            numbers.Sort();
            //No value below 1
            int expectedNumber = 1;
            foreach (int number in numbers)
            {
                if (number > expectedNumber)
                {
                    return expectedNumber;
                }
                if (number == expectedNumber)
                {
                    expectedNumber++;
                }
            }
            return expectedNumber;
        }

        private static void AddNew(TodoEntry toDoEntry)
        {
            //Receive stored values
            List<TodoEntry> toDoEntries = TodoEntry.Get();

            //Set ID
            toDoEntry.Id = GetLowestID((List<int>)toDoEntries.Select(x => x.Id).ToList());

            toDoEntry.Caption = $"Aufgabe {toDoEntry.Id}";
            toDoEntry.Description = $"Beschreibung {toDoEntry.Id}";

            toDoEntries.Add(toDoEntry);

            TodoEntry.SerializeToDoEntry(toDoEntries);
        }

    }
}
