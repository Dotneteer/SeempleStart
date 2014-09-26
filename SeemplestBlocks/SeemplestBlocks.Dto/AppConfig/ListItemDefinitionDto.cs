namespace Evolution.BuildingBlocks.Dto.Configuration
{
    /// <summary>
    /// Egy listaelem definícióját írja le
    /// </summary>
    public class ListItemDefinitionDto
    {
        /// <summary>
        /// A listaelem azonosítója
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Rendszerelemről van szó?
        /// </summary>
        public bool IsSystemItem { get; set; }
        
        /// <summary>
        /// A listaelem képernyőn megjelenő neve
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A listaelem opcionális leírása
        /// </summary>
        public string Description { get; set; }
    }
}