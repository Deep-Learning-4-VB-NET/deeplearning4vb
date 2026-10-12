Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Condition = org.datavec.api.transform.condition.Condition
Imports Writable = org.datavec.api.writable.Writable
import static org.nd4j.shade.guava.base.Preconditions.checkArgument
import static org.nd4j.shade.guava.base.Preconditions.checkNotNull

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

Namespace org.datavec.api.transform.ops


	<Serializable>
	Public Class DispatchWithConditionOp(Of U)
		Inherits DispatchOp(Of Writable, U)
		Implements IAggregableReduceOp(Of IList(Of Writable), IList(Of U))


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @NonNull private java.util.List<org.datavec.api.transform.condition.Condition> conditions;
		Private conditions As IList(Of Condition)


		Public Sub New(ByVal ops As IList(Of IAggregableReduceOp(Of Writable, IList(Of U))), ByVal conds As IList(Of Condition))
			MyBase.New(ops)
			checkNotNull(conds, "Empty condtions for a DispatchWitConditionsOp, use DispatchOp instead")

			checkArgument(ops.Count = conds.Count, "Found conditions size " & conds.Count & " expected " & ops.Count)
			conditions = conds
		End Sub

		Public Overridable Overloads Sub accept(ByVal ts As IList(Of Writable))
			Dim i As Integer = 0
			Do While i < Math.Min(MyBase.getOperations().size(), ts.Count)
				Dim cond As Condition = conditions(i)
				If cond.condition(ts) Then
					MyBase.getOperations().get(i).accept(ts(i))
				End If
				i += 1
			Loop
		End Sub

	End Class

End Namespace