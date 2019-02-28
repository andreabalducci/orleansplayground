using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public class Result
    {
        public static Result Ok = new Result();
    }


    public class Command
    {
    }

    public interface IWorker : IGrainWithStringKey
    {
        Task<Result> ExecuteAsync(Command command);
    }
}