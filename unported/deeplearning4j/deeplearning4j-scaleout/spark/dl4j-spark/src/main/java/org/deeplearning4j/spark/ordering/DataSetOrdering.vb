Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Function1 = scala.Function1
Imports Some = scala.Some
Imports Ordering = scala.math.Ordering

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

Namespace org.deeplearning4j.spark.ordering

	Public Class DataSetOrdering
		Implements Ordering(Of DataSet)

		Public Overrides Function tryCompare(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Some(Of Object)
			Return Nothing
		End Function

		Public Overrides Function compare(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Integer
			Return 0
		End Function

		Public Overrides Function lteq(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Boolean
			Return dataSet.numExamples() >= t1.numExamples()
		End Function

		Public Overrides Function gteq(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Boolean
			Return Not lteq(dataSet, t1)
		End Function

		Public Overrides Function lt(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Boolean
			Return dataSet.numExamples() >= t1.numExamples()
		End Function

		Public Overrides Function gt(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Boolean
			Return Not lt(dataSet, t1)
		End Function

		Public Overrides Function equiv(ByVal dataSet As DataSet, ByVal t1 As DataSet) As Boolean
			Return dataSet.numExamples() = t1.numExamples()
		End Function

		Public Overrides Function max(ByVal dataSet As DataSet, ByVal t1 As DataSet) As DataSet
			Return If(gt(dataSet, t1), dataSet, t1)
		End Function

		Public Overrides Function min(ByVal dataSet As DataSet, ByVal t1 As DataSet) As DataSet
			Return If(max(dataSet, t1) Is dataSet, t1, dataSet)
		End Function

		Public Overrides Function reverse() As Ordering(Of DataSet)
			Return Nothing
		End Function

		Public Overrides Function [on](Of U)(ByVal function1 As Function1(Of U, DataSet)) As Ordering(Of U)
			Return Nothing
		End Function

		Public Overrides Function mkOrderingOps(ByVal dataSet As DataSet) As Ops
			Return Nothing
		End Function
	End Class

End Namespace