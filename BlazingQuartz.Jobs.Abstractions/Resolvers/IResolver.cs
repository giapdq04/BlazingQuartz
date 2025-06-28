using System;
namespace BlazeQuartz.Jobs.Abstractions.Resolvers
{
    public interface IResolver
    {
        string Resolve(string varBlock);
    }
}

