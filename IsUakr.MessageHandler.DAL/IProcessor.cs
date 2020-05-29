using System.Threading.Tasks;

namespace IsUakr.MessageHandler.DAL
{
    public interface IProcessor
    {
        Task<string> Process(string message);
    }
}
