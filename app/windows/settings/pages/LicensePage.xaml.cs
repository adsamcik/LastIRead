﻿using System;
using System.Windows;
using System.Windows.Media;
using LastIRead.resources;
using Markdig.Wpf;

namespace LastIRead.windows.settings.pages {
	/// <summary>
	///     Interaction logic for LicensePage.xaml
	/// </summary>
	public partial class LicensePage {
		public LicensePage() {
			InitializeComponent();

			var document = Markdown.ToFlowDocument(LoadLicenses());
			var brush = (SolidColorBrush) Application.Current.Resources["SystemBaseHighColorBrush"];
			var secondaryBrush = (SolidColorBrush) Application.Current.Resources["SystemBaseMediumHighColorBrush"];
			foreach (var block in document.Blocks) {
				if (block.Foreground is SolidColorBrush blockBrush) {
					if (blockBrush.Color == Colors.Black) {
						block.Foreground = brush;
						continue;
					}
				}

				block.Foreground = secondaryBrush;
			}

			FlowReader.Document = document;
		}

		private static string LoadLicenses() {
			var resources = ResourceLoader.LoadTextResource("resources.licenses.");
			return string.Join(Environment.NewLine, resources);
			//Properties.Resources.ResourceManager.
		}
	}
}