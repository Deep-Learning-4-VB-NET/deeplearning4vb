Imports System
Imports System.IO
Imports DL4VBSystemProperties = DeepLearning4VB.org.deeplearning4vb.common.config.DL4VBSystemProperties
Imports ND4VBFileUtils = DeepLearning4VB.org.nd4vb.common.util.ND4VBFileUtils

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

Namespace org.deeplearning4vb.common.util

    ''' <summary>Temporary file helpers that honour the DL4VB temporary-directory system property.</summary>
    Public NotInheritable Class DL4VBFileUtils

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Creates a new, empty temporary file in the directory given by the
        ''' <see cref="DL4VBSystemProperties.DL4VB_TEMP_DIR_PROPERTY"/> system property if set, otherwise in
        ''' the default temporary directory.
        ''' </summary>
        ''' <returns>The full path of the created file.</returns>
        Public Shared Function createTempFile(ByVal prefix As String, ByVal suffix As String) As String
            Dim p As String = JavaSystem.getProperty(DL4VBSystemProperties.DL4VB_TEMP_DIR_PROPERTY)
            Dim dir As String = If(String.IsNullOrEmpty(p), Path.GetTempPath(), p)
            Return ND4VBFileUtils.CreateUniqueTempFile(dir, prefix, suffix)
        End Function

    End Class

End Namespace
