using System.IO;

namespace sisbase.Configuration {
    public interface IConfiguration {
        string Path { get; }
        void Update();
        void Create(DirectoryInfo di);
    }
}