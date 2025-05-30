// SPDX-FileCopyrightText: 2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Illiux <newoutlook@gmail.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <Jezithyr.@gmail.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <Jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <jmaster9999@gmail.com>
// SPDX-FileCopyrightText: 2022 Júlio César Ueti <52474532+Mirino97@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <wrexbe@protonmail.com>
// SPDX-FileCopyrightText: 2024 Crotalus <Crotalus@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Numerics;
using Content.Shared._Goobstation.Wizard.Refund;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Shitcode.Wizard.Refund
{
    [GenerateTypedNameReferences]
    public sealed partial class StoreRefundWindow : DefaultWindow
    {
        private List<RefundListingData> _listings = new();
        private bool _refundDiabled;
        private string _searchText = string.Empty;

        public event Action<NetEntity>? ListingClicked;

        public event Action? RefundAllClicked;

        public StoreRefundWindow()
        {
            RobustXamlLoader.Load(this);
            SearchBar.OnTextChanged += OnSearchTextChanged;

            RefundAllButton.OnPressed += _ => RefundAllClicked?.Invoke();
        }

        public void UpdateListings(IEnumerable<RefundListingData> listings, bool refundDisabled)
        {
            _refundDiabled = refundDisabled;
            // Server COULD send these sorted but how about we just use the client to do it instead
            _listings = listings
                .OrderBy(w => w.DisplayName,
                    Comparer<string>.Create((x, y) => string.Compare(x, y, StringComparison.Ordinal)))
                .ToList();
        }

        public void Populate()
        {
            ButtonContainer.DisposeAllChildren();
            AddButtons();
        }

        private void AddButtons()
        {
            if (_refundDiabled)
            {
                RefundAllButton.Disabled = true;
                NoRefundLabel.Visible = true;
                SearchBar.Visible = false;
                NoRefundLabel.Text = Loc.GetString("store-refund-window-refund-disabled");
                return;
            }

            if (_listings.Count == 0)
            {
                RefundAllButton.Disabled = true;
                NoRefundLabel.Visible = true;
                SearchBar.Visible = false;
                NoRefundLabel.Text = Loc.GetString("store-refund-window-nothing-to-refund");
                return;
            }

            RefundAllButton.Disabled = false;
            NoRefundLabel.Visible = false;
            SearchBar.Visible = true;

            foreach (var listing in _listings)
            {
                var name = listing.DisplayName;
                var listingUid = listing.Entity;

                var currentButtonRef = new Button
                {
                    Text = name,
                    TextAlign = Label.AlignMode.Right,
                    HorizontalAlignment = HAlignment.Center,
                    VerticalAlignment = VAlignment.Center,
                    SizeFlagsStretchRatio = 1,
                    MinSize = new Vector2(340, 20),
                    ClipText = true,
                };

                currentButtonRef.OnPressed += _ => ListingClicked?.Invoke(listingUid);
                currentButtonRef.Visible = ButtonIsVisible(currentButtonRef);

                ButtonContainer.AddChild(currentButtonRef);
            }
        }

        private bool ButtonIsVisible(Button button)
        {
            return string.IsNullOrEmpty(_searchText) || button.Text == null ||
                   button.Text.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateVisibleButtons()
        {
            foreach (var child in ButtonContainer.Children)
            {
                if (child is Button button)
                    button.Visible = ButtonIsVisible(button);
            }
        }

        private void OnSearchTextChanged(LineEdit.LineEditEventArgs args)
        {
            _searchText = args.Text;

            UpdateVisibleButtons();
            // Reset scroll bar so they can see the relevant results.
            Scroll.SetScrollValue(Vector2.Zero);
        }
    }
}