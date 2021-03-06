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

using Microsoft.Identity.Core.Cache;
using Microsoft.Identity.Core.Helpers;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccessTokenCacheItemDetails : ContentPage
    {
        internal AccessTokenCacheItemDetails(MsalAccessTokenCacheItem msalAccessTokenCacheItem, 
            MsalIdTokenCacheItem msalIdTokenCacheItem)
        {
            InitializeComponent();

            clientIdLabel.Text = msalAccessTokenCacheItem.ClientId;

            credentialTypeLabel.Text = msalAccessTokenCacheItem.CredentialType;
            environmentLabel.Text = msalAccessTokenCacheItem.Environment;
            tenantIdLabel.Text = msalAccessTokenCacheItem.TenantId;

            userIdentifierLabel.Text = msalAccessTokenCacheItem.HomeAccountId;
            userAssertionHashLabel.Text = msalAccessTokenCacheItem.UserAssertionHash;

            expiresOnLabel.Text = msalAccessTokenCacheItem.ExpiresOn.ToString(CultureInfo.InvariantCulture);
            scopesLabel.Text = msalAccessTokenCacheItem.NormalizedScopes;

            cachedAtLabel.Text = CoreHelpers
                .UnixTimestampStringToDateTime(msalAccessTokenCacheItem.CachedAt)
                .ToString(CultureInfo.InvariantCulture);

            rawClientInfoLabel.Text = msalAccessTokenCacheItem.RawClientInfo;
            clientInfoUniqueIdentifierLabel.Text = msalAccessTokenCacheItem.ClientInfo.UniqueObjectIdentifier;
            clientInfoUniqueTenantIdentifierLabel.Text = msalAccessTokenCacheItem.ClientInfo.UniqueTenantIdentifier;

            secretLabel.Text = StringShortenerConverter.GetShortStr(msalAccessTokenCacheItem.Secret, 100);
        }
    }
}
