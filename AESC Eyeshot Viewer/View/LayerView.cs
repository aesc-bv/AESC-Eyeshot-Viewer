using devDept.CustomControls;
using devDept.Eyeshot;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AESC_Eyeshot_Viewer.View
{
    internal class LayerView : LayerListView
    {
        /// <summary>
        /// Update the ListView with the current Layers
        /// Excludes empty layers
        /// </summary>
        new public void SyncLayers()
        {
            base.SyncLayers();

            var emptyLayers = new List<ListViewItem>();

            foreach (var layer in Items)
            {
                var hasEntities = Workspace
                    .Blocks
                        .SelectMany(block => block.Entities)
                        .Where(entity => entity.LayerName == (layer as ListViewItem).Text)
                        .Any();

                if (!hasEntities)
                    emptyLayers.Add(layer as ListViewItem);
            }

            foreach (var layer in emptyLayers)
                Items.Remove(layer);

            if (emptyLayers.Count > 0)
                Invalidate();
        }
    }
}
