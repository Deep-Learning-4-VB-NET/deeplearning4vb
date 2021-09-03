Imports System
Imports System.Collections.Generic
Imports Iterables = org.nd4j.shade.guava.collect.Iterables
Imports Join = org.datavec.api.transform.join.Join
Imports Writable = org.datavec.api.writable.Writable
Imports org.datavec.local.transforms.functions
Imports org.nd4j.common.primitives

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
	Public Class ExecuteJoinFromCoGroupFlatMapFunctionAdapter
		Implements FlatMapFunctionAdapter(Of Pair(Of IList(Of Writable), Pair(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))), IList(Of Writable))

		Private ReadOnly join As Join

		Public Sub New(ByVal join As Join)
			Me.join = join
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> call(org.nd4j.common.primitives.Pair<java.util.List<org.datavec.api.writable.Writable>, org.nd4j.common.primitives.Pair<java.util.List<java.util.List<org.datavec.api.writable.Writable>>, java.util.List<java.util.List<org.datavec.api.writable.Writable>>>> t2) throws Exception
		Public Overridable Function [call](ByVal t2 As Pair(Of IList(Of Writable), Pair(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable))))) As IList(Of IList(Of Writable))

			Dim leftList As IEnumerable(Of IList(Of Writable)) = t2.Second.First
			Dim rightList As IEnumerable(Of IList(Of Writable)) = t2.Second.Second

			Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim jt As Join.JoinType = join.getJoinType()
			Select Case jt
				Case Join.JoinType.Inner
					'Return records where key columns appear in BOTH
					'So if no values from left OR right: no return values
					For Each jvl As IList(Of Writable) In leftList
						For Each jvr As IList(Of Writable) In rightList
							Dim joined As IList(Of Writable) = join.joinExamples(jvl, jvr)
							ret.Add(joined)
						Next jvr
					Next jvl
				Case Join.JoinType.LeftOuter
					'Return all records from left, even if no corresponding right value (NullWritable in that case)
					For Each jvl As IList(Of Writable) In leftList
						If Iterables.size(rightList) = 0 Then
							Dim joined As IList(Of Writable) = join.joinExamples(jvl, Nothing)
							ret.Add(joined)
						Else
							For Each jvr As IList(Of Writable) In rightList
								Dim joined As IList(Of Writable) = join.joinExamples(jvl, jvr)
								ret.Add(joined)
							Next jvr
						End If
					Next jvl
				Case Join.JoinType.RightOuter
					'Return all records from right, even if no corresponding left value (NullWritable in that case)
					For Each jvr As IList(Of Writable) In rightList
						If Iterables.size(leftList) = 0 Then
							Dim joined As IList(Of Writable) = join.joinExamples(Nothing, jvr)
							ret.Add(joined)
						Else
							For Each jvl As IList(Of Writable) In leftList
								Dim joined As IList(Of Writable) = join.joinExamples(jvl, jvr)
								ret.Add(joined)
							Next jvl
						End If
					Next jvr
				Case Join.JoinType.FullOuter
					'Return all records, even if no corresponding left/right value (NullWritable in that case)
					If Iterables.size(leftList) = 0 Then
						'Only right values
						For Each jvr As IList(Of Writable) In rightList
							Dim joined As IList(Of Writable) = join.joinExamples(Nothing, jvr)
							ret.Add(joined)
						Next jvr
					ElseIf Iterables.size(rightList) = 0 Then
						'Only left values
						For Each jvl As IList(Of Writable) In leftList
							Dim joined As IList(Of Writable) = join.joinExamples(jvl, Nothing)
							ret.Add(joined)
						Next jvl
					Else
						'Records from both left and right
						For Each jvl As IList(Of Writable) In leftList
							For Each jvr As IList(Of Writable) In rightList
								Dim joined As IList(Of Writable) = join.joinExamples(jvl, jvr)
								ret.Add(joined)
							Next jvr
						Next jvl
					End If
			End Select

			Return ret
		End Function
	End Class

End Namespace