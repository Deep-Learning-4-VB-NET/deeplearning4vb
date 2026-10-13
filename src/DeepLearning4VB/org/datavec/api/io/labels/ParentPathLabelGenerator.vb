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

Imports System
Imports System.IO
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.io.labels

    ''' <summary>The label is the name of the directory containing the file.</summary>
    <Serializable>
    Public Class ParentPathLabelGenerator
        Implements PathLabelGenerator

        Public Sub New()
        End Sub

        Public Overridable Function getLabelForPath(ByVal path As String) As writable.Writable Implements PathLabelGenerator.getLabelForPath
            ' Label is in the directory
            Dim dirName As String = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path))
            Return New Text(dirName)
        End Function

        Public Overridable Function getLabelForPath(ByVal uri As Uri) As writable.Writable Implements PathLabelGenerator.getLabelForPath
            Return getLabelForPath(uri.LocalPath)
        End Function

        Public Overridable Function inferLabelClasses() As Boolean Implements PathLabelGenerator.inferLabelClasses
            Return True
        End Function
    End Class

End Namespace
