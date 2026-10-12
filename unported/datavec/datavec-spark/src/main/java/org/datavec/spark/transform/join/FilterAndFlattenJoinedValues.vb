Imports System
Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Join = org.datavec.api.transform.join.Join
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.spark.transform.join


	Public Class FilterAndFlattenJoinedValues
		Implements FlatMapFunction(Of JoinedValue, IList(Of Writable))

		Private ReadOnly joinType As Join.JoinType

		Public Sub New(ByVal joinType As Join.JoinType)
			Me.joinType = joinType
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<java.util.List<org.datavec.api.writable.Writable>> call(JoinedValue joinedValue) throws Exception
		Public Overrides Function [call](ByVal joinedValue As JoinedValue) As IEnumerator(Of IList(Of Writable))
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
				Return Collections.singletonList(joinedValue.getValues()).GetEnumerator()
			Else
				Return Collections.emptyIterator()
			End If
		End Function

	End Class

End Namespace