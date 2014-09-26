namespace Evolution.BuildingBlocks.Dto.Configuration
{
    /// <summary>
    /// Egy lokalizált erőforrás adatai
    /// </summary>
    public class LocalizedResourceDto
    {
        /// <summary>
        /// Az erőforrás kultúrkör információja 
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Az erőforrás kategóriája
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Az erőforrás kulcsa
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Az erőforrás értéke
        /// </summary>
        public string Value { get; set; }
    }
}