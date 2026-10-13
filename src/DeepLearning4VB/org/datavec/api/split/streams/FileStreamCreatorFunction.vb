Imports System
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.util.files
Imports DeepLearning4VB.org.nd4j.common.base
Imports DeepLearning4VB.org.nd4j.common.function

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

Namespace org.datavec.api.split.streams

    ''' <summary>Opens a read stream for a <c>file:</c> URI; the default stream creator of a <see cref="org.datavec.api.split.StreamInputSplit"/>.</summary>
    <Serializable>
    Public Class FileStreamCreatorFunction
        Implements [Function](Of Uri, Stream)

        Public Overridable Function apply(ByVal uri As Uri) As Stream Implements [Function](Of Uri, Stream).apply
            Dim scheme As String = If(uri.IsAbsoluteUri, uri.Scheme, Nothing)
            Preconditions.checkState(scheme Is Nothing OrElse scheme.Equals("file", StringComparison.OrdinalIgnoreCase),
                "Attempting to open URI that is not a File URI; for other stream types, you must use an appropriate stream loader function. URI: %s", uri)
            Try
                Return New FileStream(URIUtil.toFilePath(uri), FileMode.Open, FileAccess.Read)
            Catch e As IOException
                Throw New InvalidOperationException("Error loading stream for file: " & uri.ToString(), e)
            End Try
        End Function

    End Class

End Namespace
