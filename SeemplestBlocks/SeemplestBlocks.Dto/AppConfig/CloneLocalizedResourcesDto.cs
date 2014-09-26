namespace Evolution.BuildingBlocks.Dto.Configuration
{
    /// <summary>
    /// Ez az osztály az erőforrások másolásához tartozó kérést írja le
    /// </summary>
    public class CloneLocalizedResourcesDto
    {
        /// <summary>
        /// A cél nyelvi környezet kódja
        /// </summary>
        public string TargetCode { get; set; }

        /// <summary>
        /// Annak a nyelvi környezetnek a kódja, amelyet másolni kell
        /// </summary>
        public string BaseCode { get; set; }

        /// <summary>
        /// A lemásolt erőforrások alapértelmezett értéke
        /// </summary>
        public string DefaultResourceValue { get; set; }

        /// <summary>
        /// Felülírjuk a már létező erőforrásokat?
        /// </summary>
        public bool OverrideExistingResources { get; set; }
    }
}