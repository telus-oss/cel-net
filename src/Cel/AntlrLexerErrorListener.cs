﻿// Copyright 2023 TELUS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Antlr4.Runtime;

namespace Cel;

internal class AntlrLexerErrorListener : IAntlrErrorListener<int>
{
    public List<string> ErrorList { get; } = new();

    public AntlrLexerErrorListener() { }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var message = $"Line {line}:{charPositionInLine} {msg}";

        ErrorList.Add(message);
    }
}