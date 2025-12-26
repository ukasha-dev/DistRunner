// <copyright file="IFileProvider.cs" company="Your Company">
// Copyright 2024 Your Company
// Licensed under the Apache License, Version 2.0 (the "License");
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>Your Name</author>
// <site>https://Your Company.solutions/</site>

using System.Threading.Tasks;

namespace DISTRUNNER.Core.Contracts;

public interface IFileProvider
{
    void Copy(string sourceFileName, string destFileName, bool overwrite);

    void WriteAllText(string path, string contents);

    string ReadAllText(string path);

    Task WriteAllTextAsync(string path, string contents);

    Task<string> ReadAllTextAsync(string path);

    void Delete(string path);

    bool IsWithExtension(string filePath, string extension);

    void CreateZip(string sourceDirectoryPath, string zipPath);

    void ExtractZip(string zipPath, string extractPath);

    byte[] ReadAllBytes(string filePath);

    bool Exists(string filePath);

    void WriteAllBytes(string filePath, byte[] fileData);
}