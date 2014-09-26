namespace Evolution.BuildingBlocks.Dto.Configuration
{
    /// <summary>
    /// Egy nyelvi környezetet ír le
    /// </summary>
    public class LocaleDto
    {
        /// <summary>
        /// A környezet egyedi kódja (pl. "hu", "en-us" stb.)
        /// </summary>
        /// <remarks>A "def" azonosítójú környezetet speciálisan kezeljük.</remarks>
        public string Code { get; set; }

        /// <summary>
        /// A környezet egyedi megnevezése
        /// </summary>
        public string DisplayName { get; set; }
    }
}