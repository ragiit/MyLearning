namespace Apple.Web.Service.IService
{
    // Interface ini mendefinisikan kontrak untuk penyedia token.
    // Tujuannya adalah untuk mengabstraksi cara token disimpan dan diambil.
    public interface ITokenProvider
    {
        // Menyimpan token.
        void SetToken(string token);

        // Mengambil token yang tersimpan.
        string? GetToken();

        // Menghapus token.
        void ClearToken();
    }
}