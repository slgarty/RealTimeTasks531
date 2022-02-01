using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTasks.Data
{
    public class TaskItemRepository
    {
        private readonly string _connectionString;
        public TaskItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddTask(TaskItem task)
        {
            var ctx = new UserTaskItemsContext(_connectionString);
            ctx.TaskItems.Add(task);
            ctx.SaveChanges();
        }
        public void DoTask(int userId, int taskId )
        {
            var ctx = new UserTaskItemsContext(_connectionString);
            ctx.Database.ExecuteSqlInterpolated($"UPDATE TaskItems SET HandledBy = {userId} WHERE Id = {taskId}");

        }
        public void CompleteTask(int taskId)
        {
            var ctx = new UserTaskItemsContext(_connectionString);
            ctx.Database.ExecuteSqlInterpolated($"UPDATE TaskItems SET isCompleted = 1 WHERE Id = {taskId}");
        }
        public List<TaskItem> GetTaskItems()
        {
            var ctx = new UserTaskItemsContext(_connectionString);
            return ctx.TaskItems.Where(t => !t.IsCompleted).Include(t=>t.User).ToList();
        }
        public TaskItem GetById(int id)
        {
            using var ctx = new UserTaskItemsContext(_connectionString);
            return ctx.TaskItems.Include(t => t.User).FirstOrDefault(i => i.Id == id);
        }
    }
}
