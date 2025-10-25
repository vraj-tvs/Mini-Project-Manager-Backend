using ProjectManagerAPI.DTOs;

namespace ProjectManagerAPI.Services
{
    public interface ISchedulingService
    {
        ScheduleResponseDto ScheduleTasks(ScheduleRequestDto request);
    }

    public class SchedulingService : ISchedulingService
    {
        public ScheduleResponseDto ScheduleTasks(ScheduleRequestDto request)
        {
            var tasks = request.Tasks.ToList();
            var recommendedOrder = new List<string>();

            // Create a graph to represent task dependencies
            var taskMap = tasks.ToDictionary(t => t.Title, t => t);
            var inDegree = new Dictionary<string, int>();
            var graph = new Dictionary<string, List<string>>();

            // Initialize in-degree and graph
            foreach (var task in tasks)
            {
                inDegree[task.Title] = 0;
                graph[task.Title] = new List<string>();
            }

            // Build the dependency graph
            foreach (var task in tasks)
            {
                foreach (var dependency in task.Dependencies)
                {
                    if (taskMap.ContainsKey(dependency))
                    {
                        graph[dependency].Add(task.Title);
                        inDegree[task.Title]++;
                    }
                }
            }

            // Topological sort using Kahn's algorithm
            var queue = new Queue<string>();
            
            // Add tasks with no dependencies to the queue
            foreach (var kvp in inDegree)
            {
                if (kvp.Value == 0)
                {
                    queue.Enqueue(kvp.Key);
                }
            }

            while (queue.Count > 0)
            {
                var currentTask = queue.Dequeue();
                recommendedOrder.Add(currentTask);

                // Reduce in-degree for dependent tasks
                foreach (var dependent in graph[currentTask])
                {
                    inDegree[dependent]--;
                    if (inDegree[dependent] == 0)
                    {
                        queue.Enqueue(dependent);
                    }
                }
            }

            // Check for circular dependencies
            if (recommendedOrder.Count != tasks.Count)
            {
                throw new InvalidOperationException("Circular dependencies detected in task scheduling");
            }

            return new ScheduleResponseDto
            {
                RecommendedOrder = recommendedOrder.ToArray()
            };
        }
    }
}
