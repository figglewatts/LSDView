using System;
using CommandLine;

namespace LSDView.Headless
{
    public interface IHeadlessCommand
    {
        void Register(ref ParserResult<object> parserResult);

        Type OptionsType { get; }
    }
}
