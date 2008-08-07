using Microsoft.SharePoint;
using System;
using Microsoft.SharePoint.Workflow;

namespace DocVersion
{
    class DocVersionEventReciever : SPItemEventReceiver
    {
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            DisableEventFiring(); // ValidateItem will fire an event
            ValidateItem(properties);
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            DisableEventFiring(); // ValidateItem will fire an event
            ValidateItem(properties);
        }

        protected bool ValidateItem(SPItemEventProperties properties)
        {
            SPSite siteV = null;
            SPWeb webV = null;

            if (properties.ListItemId > 0 && properties.ListId != Guid.Empty)
            {
                try
                {
                    siteV = new SPSite(properties.WebUrl);
                    webV = siteV.OpenWeb();
                    SPList spList = webV.Lists.GetList(properties.ListId, false);
                    SPListItem Item = spList.GetItemById(properties.ListItemId);
                    SPListItemVersionCollection ItemVersions = Item.Versions;

                    Item["WSSVersion"] = Item["_UIVersionString"];
                    Item["WSSAuthor"] = Item.File.Versions[Item.File.Versions.Count - 1].CreatedBy.Name;
                    Item["WSSComment"] = Item.File.Versions[Item.File.Versions.Count - 1].CheckInComment;
                    for (int i = 0; (i <= 1) && (i <= (ItemVersions.Count - 2)); i++)
                    {
                        SPFile newFile = webV.GetFile(Item.Url);
                        Item["WSSVer" + i + "Num"] = ItemVersions[i + 1].VersionLabel;
                        Item["WSSVer" + i + "Author"] = newFile.Versions[newFile.Versions.Count - (i + 1)].CreatedBy.Name;
                        Item["WSSVer" + i + "Comment"] = newFile.Versions[newFile.Versions.Count - (i + 1)].CheckInComment;
                    }
                    Item.SystemUpdate();
                }
                catch
                {
                    // Do Nothing really...
                }
            }
            return true;
        }
    }
}


