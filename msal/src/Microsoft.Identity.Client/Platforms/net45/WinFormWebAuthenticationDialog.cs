﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Core.UI;

namespace Microsoft.Identity.Client.Internal.UI
{
    /// <summary>
    /// The browser dialog used for user authentication
    /// </summary>
    [ComVisible(true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WindowsFormsWebAuthenticationDialog : WindowsFormsWebAuthenticationDialogBase
    {
        private int statusCode;
        private bool zoomed;

        /// <summary>
        /// Default constructor
        /// </summary>
        public WindowsFormsWebAuthenticationDialog(object ownerWindow)
            : base(ownerWindow)
        {
            Shown += FormShownHandler;
            WebBrowser.DocumentTitleChanged += WebBrowserDocumentTitleChangedHandler;
            WebBrowser.ObjectForScripting = this;
        }

        /// <summary>
        /// </summary>
        protected override void OnAuthenticate()
        {
            zoomed = false;
            statusCode = 0;
            ShowBrowser();

            base.OnAuthenticate();
        }

        /// <summary>
        /// </summary>
        public void ShowBrowser()
        {
            DialogResult uiResult = DialogResult.None;
            InvokeHandlingOwnerWindow(() => uiResult = ShowDialog(ownerWindow));

            switch (uiResult)
            {
                case DialogResult.OK:
                    break;
                case DialogResult.Cancel:
                    Result = new AuthorizationResult(AuthorizationStatus.UserCancel, null);
                    break;
                default:
                    throw CreateExceptionForAuthenticationUiFailed(statusCode);
            }
        }

        /// <summary>
        /// </summary>
        protected override void WebBrowserNavigatingHandler(object sender, WebBrowserNavigatingEventArgs e)
        {
            SetBrowserZoom();
            base.WebBrowserNavigatingHandler(sender, e);
        }

        /// <summary>
        /// </summary>
        protected override void OnClosingUrl()
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// </summary>
        protected override void OnNavigationCanceled(int inputStatusCode)
        {
            statusCode = inputStatusCode;
            DialogResult = (inputStatusCode == 0) ? DialogResult.Cancel : DialogResult.Abort;
        }

        private void SetBrowserZoom()
        {
            int windowsZoomPercent = DpiHelper.ZoomPercent;
            if (NativeWrapper.NativeMethods.IsProcessDPIAware() && 100 != windowsZoomPercent && !zoomed)
            {
                // There is a bug in some versions of the IE browser control that causes it to 
                // ignore scaling unless it is changed.
                SetBrowserControlZoom(windowsZoomPercent - 1);
                SetBrowserControlZoom(windowsZoomPercent);

                zoomed = true;
            }
        }

        private void SetBrowserControlZoom(int zoomPercent)
        {
            NativeWrapper.IWebBrowser2 browser2 = (NativeWrapper.IWebBrowser2) WebBrowser.ActiveXInstance;
            NativeWrapper.IOleCommandTarget cmdTarget = browser2.Document as NativeWrapper.IOleCommandTarget;
            if (cmdTarget != null)
            {
                const int OLECMDID_OPTICAL_ZOOM = 63;
                const int OLECMDEXECOPT_DONTPROMPTUSER = 2;

                object[] commandInput = {zoomPercent};

                int hResult = cmdTarget.Exec(
                    IntPtr.Zero, OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT_DONTPROMPTUSER, commandInput, IntPtr.Zero);
                Marshal.ThrowExceptionForHR(hResult);
            }
        }

        private void FormShownHandler(object sender, EventArgs e)
        {
            // If we don't have an owner we need to make sure that the pop up browser 
            // window is on top of other windows.  Activating the window will accomplish this.
            if (null == Owner)
            {
                Activate();
            }
        }

        private void WebBrowserDocumentTitleChangedHandler(object sender, EventArgs e)
        {
            Text = WebBrowser.DocumentTitle;
        }
    }
}