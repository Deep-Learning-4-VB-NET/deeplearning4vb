Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports DataAction = org.datavec.api.transform.DataAction
Imports Transform = org.datavec.api.transform.Transform
Imports Condition = org.datavec.api.transform.condition.Condition
Imports Filter = org.datavec.api.transform.filter.Filter
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports SequenceComparator = org.datavec.api.transform.sequence.SequenceComparator
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.serde


	Public Class ListWrappers

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public static class TransformList
		Public Class TransformList
			Friend list As IList(Of Transform)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TransformList(@JsonProperty("list") java.util.List<org.datavec.api.transform.Transform> list)
			Public Sub New(ByVal list As IList(Of Transform))
				Me.list = list
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public static class FilterList
		Public Class FilterList
			Friend list As IList(Of Filter)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FilterList(@JsonProperty("list") java.util.List<org.datavec.api.transform.filter.Filter> list)
			Public Sub New(ByVal list As IList(Of Filter))
				Me.list = list
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public static class ConditionList
		Public Class ConditionList
			Friend list As IList(Of Condition)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConditionList(@JsonProperty("list") java.util.List<org.datavec.api.transform.condition.Condition> list)
			Public Sub New(ByVal list As IList(Of Condition))
				Me.list = list
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public static class ReducerList
		Public Class ReducerList
			Friend list As IList(Of IAssociativeReducer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReducerList(@JsonProperty("list") java.util.List<org.datavec.api.transform.reduce.IAssociativeReducer> list)
			Public Sub New(ByVal list As IList(Of IAssociativeReducer))
				Me.list = list
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public static class SequenceComparatorList
		Public Class SequenceComparatorList
			Friend list As IList(Of SequenceComparator)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceComparatorList(@JsonProperty("list") java.util.List<org.datavec.api.transform.sequence.SequenceComparator> list)
			Public Sub New(ByVal list As IList(Of SequenceComparator))
				Me.list = list
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public static class DataActionList
		Public Class DataActionList
			Friend list As IList(Of DataAction)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DataActionList(@JsonProperty("list") java.util.List<org.datavec.api.transform.DataAction> list)
			Public Sub New(ByVal list As IList(Of DataAction))
				Me.list = list
			End Sub
		End Class
	End Class

End Namespace