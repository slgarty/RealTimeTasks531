using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using RealTimeTasks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTasks.Web
{
    public class TaskHub : Hub
    {
        private readonly string _connectionString;

        public TaskHub(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public void NewTask(string title)
        {
            var taskrepo = new TaskItemRepository(_connectionString);
            var task = new TaskItem { Title = title, IsCompleted = false };
            taskrepo.AddTask(task);
            SendTasks();
        }

        private void SendTasks()
        {
            var taskrepo = new TaskItemRepository(_connectionString);
            var tasks = taskrepo.GetTaskItems();

            Clients.All.SendAsync("RenderTasks", tasks.Select(t => new
            {
                Id = t.Id,
                Title = t.Title,
                HandledBy = t.HandledBy,
                UserDoingIt = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : null
            }));
        }

        public void GetAllTasks()
        {
            SendTasks();
        }

        public void SetDoing(int taskId)
        {
            var repo = new UserRepository(_connectionString);
            var user = repo.GetByEmail(Context.User.Identity.Name);
            var taskrepo = new TaskItemRepository(_connectionString);
            taskrepo.DoTask(user.Id,taskId);
            SendTasks();
        }
        public void SetDone(int taskId)
        {
            var taskRepo = new TaskItemRepository(_connectionString);
            taskRepo.CompleteTask(taskId);
            SendTasks();
        }
    }
}
