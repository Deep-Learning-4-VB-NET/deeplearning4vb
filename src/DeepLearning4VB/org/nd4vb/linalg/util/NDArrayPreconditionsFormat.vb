Imports System
Imports System.Collections.Generic
Imports SysMath = System.Math
Imports PreconditionsFormat = DeepLearning4VB.org.nd4vb.common.base.PreconditionsFormat
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports NDArrayIndex = DeepLearning4VB.org.nd4vb.linalg.indexing.NDArrayIndex

'
' *  ******************************************************************************
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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' The <c>Preconditions</c> message tags for arrays: <c>%ndRank</c>, <c>%ndShape</c>, <c>%ndStride</c>,
    ''' <c>%ndLength</c>, <c>%ndSInfo</c> (shape information without line breaks) and <c>%nd10</c> (the first 10 values).
    ''' Java registers it through <c>ServiceLoader</c>; here it must be registered with
    ''' <c>Preconditions.registerFormat(New NDArrayPreconditionsFormat())</c>, which the ND4VB factory does when it starts.
    ''' </summary>
    ''' @author Alex Black
    Public Class NDArrayPreconditionsFormat
        Implements PreconditionsFormat

        Private Shared ReadOnly TAGS As IList(Of String) = New List(Of String) From {"%ndRank", "%ndShape", "%ndStride", "%ndLength", "%ndSInfo", "%nd10"}

        Public Overridable Function formatTags() As IList(Of String) Implements PreconditionsFormat.formatTags
            Return TAGS
        End Function

        Public Overridable Function format(ByVal tag As String, ByVal arg As Object) As String Implements PreconditionsFormat.format
            If arg Is Nothing Then
                Return "null"
            End If
            Dim arr As INDArray = DirectCast(arg, INDArray)
            Select Case tag
                Case "%ndRank"
                    Return arr.rank().ToString()
                Case "%ndShape"
                    Return Arrays.toString(arr.shape())
                Case "%ndStride"
                    Return Arrays.toString(arr.stride())
                Case "%ndLength"
                    Return arr.length().ToString()
                Case "%ndSInfo"
                    Return arr.shapeInfoToString().Replace(vbLf, "")
                Case "%nd10"
                    If arr.Scalar OrElse arr.Empty Then
                        Return CObj(arr).ToString()
                    End If
                    Dim [sub] As INDArray = arr.reshape(New Long() {arr.length()}).get(NDArrayIndex.interval(0L, SysMath.Min(arr.length(), 10L)))
                    Return CObj([sub]).ToString()
                Case Else
                    'Should never happen
                    Throw New System.InvalidOperationException("Unknown format tag: " & tag)
            End Select
        End Function
    End Class

End Namespace