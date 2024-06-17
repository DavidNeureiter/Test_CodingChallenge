using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using websLINE.Database;

namespace websLINE
{
    public static class MockingBase
    {
        public static DbMock<TodoEntry> TodoEntries { get; set; } = new DbMock<TodoEntry>();

        static MockingBase()
        {
            LoadTodoEntries();
        }

        private static void LoadTodoEntries()
        {
            TodoEntries.AddRange(TodoEntry.Get());
        }

        public static void AddTodo()
        {

        }

        public static void LoadToDo()
        {

        }
    }

    public class DbMock<T> where T : IDbObject
    {

        private readonly List<T> entries = new List<T>();

        public void Add(T entry)
        {
            if (entry.Id != 0 && entries.Any(e => e.Id == entry.Id))
            {
                return;
            }

            entries.Add(entry);
        }

        public void AddRange(List<T> entries)
        {
            foreach (T entry in entries)
            {
                if (entry.Id != 0 && this.entries.Any(e => e.Id == entry.Id))
                {
                    continue;
                }
                this.entries.Add(entry);
            }

        }


        public void Delete(T entry)
        {
            entry.Deleted = true;
        }

        public T? Get(int id)
        {
            
            return entries.FirstOrDefault(e => e.Id == id);
        }

        public List<T> Select(Func<T, bool> predicate)
        {

            return entries.Where(predicate).ToList();
        }


        public List<T> Get()
        {
            return entries;
        }





    }
}