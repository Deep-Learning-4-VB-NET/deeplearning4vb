Imports Value = lombok.Value
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.mdp.toy

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public class SimpleToyState implements org.deeplearning4j.rl4j.space.Encodable
	Public Class SimpleToyState
		Implements Encodable

		Friend i As Integer
		Friend [step] As Integer

		Public Overridable Function toArray() As Double() Implements Encodable.toArray
			Dim ar(0) As Double
			ar(0) = (20 - i)
			Return ar
		End Function

		Public Overridable ReadOnly Property Skipped As Boolean Implements Encodable.isSkipped
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property Data As INDArray Implements Encodable.getData
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function dup() As Encodable
			Return Nothing
		End Function
	End Class

End Namespace