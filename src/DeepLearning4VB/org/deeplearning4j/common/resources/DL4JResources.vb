Imports System
Imports System.IO
Imports DL4JSystemProperties = DeepLearning4VB.org.deeplearning4j.common.config.DL4JSystemProperties
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.deeplearning4j.common.resources

    ''' <summary>
    ''' Locations of DL4J's downloadable resources: the base download URL and the local base directory
    ''' (default <c>~/.deeplearning4j</c>). Directories are path strings (the Java original used java.io.File).
    ''' </summary>
    Public NotInheritable Class DL4JResources

        <Obsolete("Use DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY")>
        Public Const DL4J_RESOURCES_DIR_PROPERTY As String = DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY
        <Obsolete("Use DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY")>
        Public Const DL4J_BASE_URL_PROPERTY As String = DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY
        Private Const DL4J_DEFAULT_URL As String = "https://dl4jdata.blob.core.windows.net/"

        Private Shared baseDirectoryValue As String
        Private Shared baseURL As String

        Shared Sub New()
            resetBaseDirectoryLocation()
            Dim [property] As String = JavaSystem.getProperty(DL4JSystemProperties.DL4J_RESOURCES_BASE_URL_PROPERTY)
            baseURL = If([property], DL4J_DEFAULT_URL)
        End Sub

        Private Sub New()
        End Sub

        ''' <summary>The base URL that resources are downloaded from.</summary>
        Public Shared Property BaseDownloadURL As String
            Get
                Return baseURL
            End Get
            Set(ByVal value As String)
                Preconditions.checkNotNull(value, "Base download URL must not be null")
                baseURL = value
            End Set
        End Property

        Public Shared Sub setBaseDownloadURL(ByVal baseDownloadURL As String)
            DL4JResources.BaseDownloadURL = baseDownloadURL
        End Sub

        Public Shared Function getBaseDownloadURL() As String
            Return baseURL
        End Function

        ''' <summary>The download URL for a path relative to the base URL.</summary>
        Public Shared Function getURL(ByVal relativeToBase As String) As Uri
            Return New Uri(getURLString(relativeToBase))
        End Function

        Public Shared Function getURLString(ByVal relativeToBase As String) As String
            If relativeToBase.StartsWith("/", StringComparison.Ordinal) Then
                relativeToBase = relativeToBase.Substring(1)
            End If
            Return baseURL & relativeToBase
        End Function

        ''' <summary>Re-reads the base directory from the system property (or uses ~/.deeplearning4j) and creates it.</summary>
        Public Shared Sub resetBaseDirectoryLocation()
            Dim [property] As String = JavaSystem.getProperty(DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY)
            If [property] IsNot Nothing Then
                baseDirectoryValue = [property]
            Else
                baseDirectoryValue = Path.Combine(JavaSystem.getProperty("user.home"), ".deeplearning4j")
            End If
            Directory.CreateDirectory(baseDirectoryValue)
        End Sub

        ''' <summary>The local base directory under which resources are stored; it must exist when set.</summary>
        Public Shared Property BaseDirectory As String
            Get
                Return baseDirectoryValue
            End Get
            Set(ByVal value As String)
                Preconditions.checkNotNull(value, "Base directory must not be null")
                Preconditions.checkState(Directory.Exists(value), "Specified base directory does not exist and/or is not a directory: %s", value)
                baseDirectoryValue = Path.GetFullPath(value)
            End Set
        End Property

        Public Shared Sub setBaseDirectory(ByVal f As String)
            DL4JResources.BaseDirectory = f
        End Sub

        Public Shared Function getBaseDirectory() As String
            Return baseDirectoryValue
        End Function

        ''' <summary>Returns (and creates) the directory for a named resource of the given type.</summary>
        Public Shared Function getDirectory(ByVal resourceType As ResourceType, ByVal resourceName As String) As String
            Dim f As String = Path.Combine(baseDirectoryValue, resourceType.resourceName(), resourceName)
            Directory.CreateDirectory(f)
            Return f
        End Function
    End Class

End Namespace
