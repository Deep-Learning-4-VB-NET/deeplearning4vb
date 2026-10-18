Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.primitives
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
    ''' The array-free part of upstream <c>org.nd4j.linalg.shape.ShapeTests</c>. The other twelve tests (slices, tensors
    ''' along dimensions, reductions, ...) need arrays and move with the array tests (phase 2 wave 3).
    ''' </summary>
    <TestClass>
    Public Class ShapeTests

        <TestMethod>
        Public Sub testBroadcastShapes()
            'Test cases: in1Shape, in2Shape, shapeOf(op(in1,in2))
            Dim testCases As IList(Of Triple(Of Long(), Long(), Long())) = New List(Of Triple(Of Long(), Long(), Long()))()
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 1}, New Long() {1, 4}, New Long() {3, 4}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 1}, New Long() {3, 4}, New Long() {3, 4}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 4}, New Long() {1, 4}, New Long() {3, 4}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 4, 1}, New Long() {1, 1, 5}, New Long() {3, 4, 5}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 4, 1}, New Long() {3, 1, 5}, New Long() {3, 4, 5}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 1, 5}, New Long() {1, 4, 1}, New Long() {3, 4, 5}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 1, 5}, New Long() {1, 4, 5}, New Long() {3, 4, 5}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 1, 5}, New Long() {3, 4, 5}, New Long() {3, 4, 5}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {3, 1, 1, 1}, New Long() {1, 4, 5, 6}, New Long() {3, 4, 5, 6}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {1, 1, 1, 6}, New Long() {3, 4, 5, 6}, New Long() {3, 4, 5, 6}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {1, 4, 5, 1}, New Long() {3, 1, 1, 6}, New Long() {3, 4, 5, 6}))
            testCases.Add(New Triple(Of Long(), Long(), Long())(New Long() {1, 6}, New Long() {3, 4, 5, 1}, New Long() {3, 4, 5, 6}))

            For Each t As Triple(Of Long(), Long(), Long()) In testCases
                Dim x() As Long = t.getFirst()
                Dim y() As Long = t.getSecond()
                Dim exp() As Long = t.getThird()

                Dim act() As Long = ShapeT.broadcastOutputShape(x, y)
                assertArrayEquals(exp, act)
            Next t
        End Sub

    End Class

End Namespace
