﻿
namespace BlazeQuartz.Services
{
    public interface IJobUIProvider
    {
        Type GetJobUIType(string? jobTypeFullName);
    }
}