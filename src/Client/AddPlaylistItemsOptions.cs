namespace Beefweb.Client
{
    /// <summary>
    /// Options for <see cref="IPlayerClient.AddPlaylistItems"/> request.
    /// </summary>
    public sealed class AddPlaylistItemsOptions
    {
        /// <summary>
        /// Add items asynchronously.
        /// This means request will immediately return and items will be added in the background.
        /// </summary>
        public bool ProcessAsynchronously { get; set; }

        /// <summary>
        /// Start playing newly added items starting from the first added item.
        /// </summary>
        public bool PlayAddedItems { get; set; }

        /// <summary>
        /// Replace existing items in the target playlist.
        /// If this option is true, <see cref="TargetPosition"/> has no effect.
        /// </summary>
        public bool ReplaceExistingItems { get; set; }

        /// <summary>
        /// Insert items at the specified position in the playlist.
        /// By default items are added to the end of the playlist.
        /// </summary>
        public int? TargetPosition { get; set; }
    }
}
