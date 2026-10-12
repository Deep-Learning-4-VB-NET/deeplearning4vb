Imports Getter = lombok.Getter
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.rl4j.space

	Public Class DiscreteSpace
		Implements ActionSpace(Of Integer)

		'size of the space also defined as the number of different actions
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final int size;
		Protected Friend ReadOnly size As Integer
		Protected Friend ReadOnly rnd As Random

		Public Sub New(ByVal size As Integer)
			Me.New(size, Nd4j.Random)
		End Sub

		Public Sub New(ByVal size As Integer, ByVal rnd As Random)
			Me.size = size
			Me.rnd = rnd
		End Sub

		Public Overridable Function randomAction() As Integer?
			Return rnd.nextInt(size)
		End Function

		Public Overridable Function encode(ByVal a As Integer?) As Object
			Return a
		End Function

		Public Overridable Function noOp() As Integer?
			Return 0
		End Function

	End Class

End Namespace