using System;
using System.Threading.Tasks;

namespace CorrugatedIron.Comms
{
    public interface IRiakNode : IDisposable
    {   
        Task<RiakPbcSocket> CreateSocket();
        Task Release(RiakPbcSocket socket);
        Task ReleaseAll();

        Task GetSingleResultViaPbc(Func<RiakPbcSocket, Task> useFun);
        Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun);
        Task GetSingleResultViaPbc(RiakPbcSocket socket, Func<RiakPbcSocket, Task> useFun);
        Task<TResult> GetSingleResultViaPbc<TResult>(RiakPbcSocket socket, Func<RiakPbcSocket, Task<TResult>> useFun);
        Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun);
        Task GetMultipleResultViaPbc(RiakPbcSocket socket, Func<RiakPbcSocket, Task> useFun);

        Task GetSingleResultViaRest(Func<string, Task> useFun);
        Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun);
        Task GetMultipleResultViaRest(Func<string, Task> useFun);
    }
}