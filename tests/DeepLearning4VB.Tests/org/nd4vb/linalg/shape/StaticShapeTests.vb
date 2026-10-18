Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

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

Namespace org.nd4vb.linalg.shape

    ''' <summary>
    ''' The array-free part of upstream <c>org.nd4j.linalg.shape.StaticShapeTests</c>. Java only timed and printed the
    ''' conversions; this port asserts the printed values. <c>testBufferToIntShapeStrideMethods</c> needs the test arrays
    ''' of <c>NDArrayCreationUtil</c> and moves with the array tests (phase 2 wave 3).
    ''' </summary>
    <TestClass>
    Public Class StaticShapeTests

        <TestMethod>
        Public Sub testShapeInd2Sub()
            For i As Integer = 0 To 999
                ShapeT.ind2subC(New Integer() {2, 2}, 1)
            Next i

            assertEquals("C [0, 1]", "C " & Arrays.toString(ShapeT.ind2subC(New Integer() {2, 2}, 1)))
            assertEquals("F [1, 0]", "F " & Arrays.toString(ShapeT.ind2sub(New Integer() {2, 2}, 1)))
        End Sub

    End Class

End Namespace
