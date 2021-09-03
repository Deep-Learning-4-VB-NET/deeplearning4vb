Imports System
Imports System.Collections.Generic
Imports Join = org.datavec.api.transform.join.Join
Imports Writable = org.datavec.api.writable.Writable
Imports org.datavec.local.transforms.functions

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

Namespace org.datavec.local.transforms.join


	<Serializable>
	Public Class FilterAndFlattenJoinedValuesAdapter
		Implements FlatMapFunctionAdapter(Of JoinedValue, IList(Of Writable))

		Private ReadOnly joinType As Join.JoinType

		Public Sub New(ByVal joinType As Join.JoinType)
			Me.joinType = joinType
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> call(JoinedValue joinedValue) throws Exception
		Public Overridable Function [call](ByVal joinedValue As JoinedValue) As IList(Of IList(Of Writable))
			Dim keep As Boolean
			Select Case joinType
				Case Join.JoinType.Inner
					'Only keep joined values where we have both left and right
					keep = joinedValue.isHaveLeft() AndAlso joinedValue.isHaveRight()
				Case Join.JoinType.LeftOuter
					'Keep all values where left is not missing/null
					keep = joinedValue.isHaveLeft()
				Case Join.JoinType.RightOuter
					'Keep all values where right is not missing/null
					keep = joinedValue.isHaveRight()
				Case Join.JoinType.FullOuter
					'Keep all values
					keep = True
				Case Else
					Throw New Exception("Unknown/not implemented join type: " & joinType)
			End Select

			If keep Then
				Return Collections.singletonList(joinedValue.getValues())
			Else
				Return Collections.emptyList()
			End If
		End Function
	End Class

End Namespace