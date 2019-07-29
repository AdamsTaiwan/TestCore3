using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AppBase0500
{
    public static class Extensions
    {

        #region find parent

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T? TryFindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            //get parent item
            DependencyObject? parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            //check if the parent matches the type we're looking for
            T? parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject? GetParentObject(this DependencyObject? child)
        {
            if (child == null)
            {
                return null;
            }

            //handle content elements separately
            ContentElement? contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }

                FrameworkContentElement? fce = contentElement as FrameworkContentElement;
                return fce?.Parent;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement? frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null)
                {
                    return parent;
                }
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        #endregion

        #region find children

        /// <summary>
        /// Analyzes both visual and logical tree in order to find all elements of a given
        /// type that are descendants of the <paramref name="source"/> item.
        /// </summary>
        /// <typeparam name="T">The type of the queried items.</typeparam>
        /// <param name="source">The root element that marks the source of the search. If the
        /// source is already of the requested type, it will not be included in the result.</param>
        /// <returns>All descendants of <paramref name="source"/> that match the requested type.</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source != null)
            {
                var childs = GetChildObjects(source);
                foreach (DependencyObject child in childs)
                {
                    //analyze if children match the requested type
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    //recurse tree
                    if (child != null)
                    {
                        foreach (T descendant in FindChildren<T>(child))
                        {
                            yield return descendant;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetChild"/> method, which also
        /// supports content elements. Keep in mind that for content elements,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="parent">The item to be processed.</param>
        /// <returns>The submitted item's child elements, if available.</returns>
        public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
        {
            if (parent == null)
            {
                yield break;
            }

            if (parent is ContentElement || parent is FrameworkElement)
            {
                //use the logical tree for content / framework elements
                foreach (object obj in LogicalTreeHelper.GetChildren(parent))
                {
                    var depObj = obj as DependencyObject;
                    if (depObj != null)
                    {
                        yield return (DependencyObject)obj;
                    }
                }
            }
            else
            {
                //use the visual tree per default
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(parent, i);
                }
            }
        }

        #endregion

        #region find from point

        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position. 
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static T? TryFindFromPoint<T>(UIElement reference, System.Windows.Point point)
            where T : DependencyObject
        {
            DependencyObject? element = reference.InputHitTest(point) as DependencyObject;

            if (element == null)
            {
                return null;
            }
            else if (element is T)
            {
                return (T)element;
            }
            else
            {
                return TryFindParent<T>(element);
            }
        }
        #endregion

        public static string GetFolder(string defaultFolder, string desc)
        {
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = desc,
                SelectedPath = defaultFolder
            };
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return folderBrowser.SelectedPath;
            }
            else
            {
                return defaultFolder;
            }
        }

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
                {
                    System.Drawing.Imaging.PropertyItem? propItem = null;
                    try
                    {
                        propItem = myImage.GetPropertyItem(36867);
                    }
                    catch { }
                    if (propItem != null)
                    {
                        string r = System.Text.Encoding.UTF8.GetString(propItem.Value);
                        string dateTaken = r.Replace(":", "");
                        return DateTime.Parse(dateTaken);
                    }
                    else
                    {
                        return new FileInfo(path).LastWriteTime;
                    }
                }
            }
        }

        /// <summary>
        /// FxCop requires all Marshalled functions to be in a class called NativeMethods.
        /// </summary>
        public static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }

        public static void DelayedFocus(this UIElement uiElement)
        {
            if (!uiElement.ToString().Contains("DoubleUpDown"))
            {
                if (!uiElement.ToString().Contains("DateTimeUpDown"))
                {
                    uiElement.Dispatcher.BeginInvoke(
                    new Action(delegate
                    {
                        uiElement.Focusable = true;
                        uiElement.Focus();
                        Keyboard.Focus(uiElement);
                    }),
                       DispatcherPriority.Render);
                }
            }
        }

        /*
        /// <summary>
        /// Convert Byte Array to Image Source
        /// </summary>
        /// <param name="source">byte array</param>
        /// <returns>ImageSource</returns>
        public static ImageSource ToImageSource(this byte[] source)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.StreamSource = new MemoryStream(source);
            b.EndInit();
            return b;
        }*/

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, (Action)(() => { }));
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        /// <summary>
        /// Convert Byte Array to Image Source
        /// </summary>
        /// <param name="source">byte array</param>
        /// <returns>ImageSource</returns>
        public static ImageSource ToImageSource(this byte[] source)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.StreamSource = new MemoryStream(source);
            b.EndInit();
            return b;
        }

    }
}
