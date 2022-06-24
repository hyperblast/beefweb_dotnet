namespace Beefweb.Client
{
    public sealed class AddPlaylistItemsOptions
    {
        public bool ProcessAsynchronously { get; set; }

        public bool PlayAddedItems { get; set; }

        public bool ReplaceExistingItems { get; set; }

        public int? TargetPosition { get; set; }
    }
}
