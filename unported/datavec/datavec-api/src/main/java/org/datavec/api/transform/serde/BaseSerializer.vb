Imports System
Imports System.Collections.Generic
Imports DataAction = org.datavec.api.transform.DataAction
Imports Transform = org.datavec.api.transform.Transform
Imports Condition = org.datavec.api.transform.condition.Condition
Imports Filter = org.datavec.api.transform.filter.Filter
Imports CalculateSortedRank = org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports SequenceComparator = org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports TypeReference = org.nd4j.shade.jackson.core.type.TypeReference
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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


	Public MustInherit Class BaseSerializer

		Public MustOverride ReadOnly Property ObjectMapper As ObjectMapper

		Private Function load(Of T)(ByVal str As String, ByVal clazz As Type(Of T)) As T
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.readValue(str, clazz)
			Catch e As Exception
				'TODO better exception
				Throw New Exception(e)
			End Try
		End Function

		Private Function load(Of T)(ByVal str As String, ByVal typeReference As TypeReference(Of T)) As T
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.readValue(str, typeReference)
			Catch e As Exception
				'TODO better exception
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Serialize the specified object, such as a <seealso cref="Transform"/>, <seealso cref="Condition"/>, <seealso cref="Filter"/>, etc<br>
		''' <b>NOTE:</b> For lists use the list methods, such as <seealso cref="serializeTransformList(List)"/><br>
		''' <para>
		''' To deserialize, use the appropriate method: <seealso cref="deserializeTransform(String)"/> for example.
		''' 
		''' </para>
		''' </summary>
		''' <param name="o"> Object to serialize </param>
		''' <returns> String (json/yaml) representation of the object </returns>
		Public Overridable Function serialize(ByVal o As Object) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(o)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		'===================================================================
		'Wrappers for arrays and lists

		Public Overridable Function serialize(ByVal transforms() As Transform) As String
			Return serializeTransformList(New List(Of Transform) From {transforms})
		End Function

		''' <summary>
		''' Serialize a list of Transforms
		''' </summary>
		Public Overridable Function serializeTransformList(ByVal list As IList(Of Transform)) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(New ListWrappers.TransformList(list))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function


		Public Overridable Function serialize(ByVal filters() As Filter) As String
			Return serializeFilterList(New List(Of Filter) From {filters})
		End Function

		''' <summary>
		''' Serialize a list of Filters
		''' </summary>
		Public Overridable Function serializeFilterList(ByVal list As IList(Of Filter)) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(New ListWrappers.FilterList(list))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function serialize(ByVal conditions() As Condition) As String
			Return serializeConditionList(New List(Of Condition) From {conditions})
		End Function

		''' <summary>
		''' Serialize a list of Conditions
		''' </summary>
		Public Overridable Function serializeConditionList(ByVal list As IList(Of Condition)) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(New ListWrappers.ConditionList(list))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function serialize(ByVal reducers() As IAssociativeReducer) As String
			Return serializeReducerList(New List(Of IAssociativeReducer) From {reducers})
		End Function

		''' <summary>
		''' Serialize a list of IReducers
		''' </summary>
		Public Overridable Function serializeReducerList(ByVal list As IList(Of IAssociativeReducer)) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(New ListWrappers.ReducerList(list))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function serialize(ByVal seqComparators() As SequenceComparator) As String
			Return serializeSequenceComparatorList(New List(Of SequenceComparator) From {seqComparators})
		End Function

		''' <summary>
		''' Serialize a list of SequenceComparators
		''' </summary>
		Public Overridable Function serializeSequenceComparatorList(ByVal list As IList(Of SequenceComparator)) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(New ListWrappers.SequenceComparatorList(list))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function serialize(ByVal dataActions() As DataAction) As String
			Return serializeDataActionList(New List(Of DataAction) From {dataActions})
		End Function

		''' <summary>
		''' Serialize a list of DataActions
		''' </summary>
		Public Overridable Function serializeDataActionList(ByVal list As IList(Of DataAction)) As String
			Dim om As ObjectMapper = ObjectMapper
			Try
				Return om.writeValueAsString(New ListWrappers.DataActionList(list))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function


		'======================================================================
		' Deserialization methods

		''' <summary>
		''' Deserialize a Transform serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the Transform </param>
		''' <returns> Transform </returns>
		Public Overridable Function deserializeTransform(ByVal str As String) As Transform
			Return load(str, GetType(Transform))
		End Function

		''' <summary>
		''' Deserialize a Filter serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the Filter </param>
		''' <returns> Filter </returns>
		Public Overridable Function deserializeFilter(ByVal str As String) As Filter
			Return load(str, GetType(Filter))
		End Function

		''' <summary>
		''' Deserialize a Condition serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the Condition </param>
		''' <returns> Condition </returns>
		Public Overridable Function deserializeCondition(ByVal str As String) As Condition
			Return load(str, GetType(Condition))
		End Function

		''' <summary>
		''' Deserialize an IStringReducer serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the IStringReducer </param>
		''' <returns> IStringReducer </returns>
		Public Overridable Function deserializeReducer(ByVal str As String) As IAssociativeReducer
			Return load(str, GetType(IAssociativeReducer))
		End Function

		''' <summary>
		''' Deserialize a SequenceComparator serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the SequenceComparator </param>
		''' <returns> SequenceComparator </returns>
		Public Overridable Function deserializeSequenceComparator(ByVal str As String) As SequenceComparator
			Return load(str, GetType(SequenceComparator))
		End Function

		''' <summary>
		''' Deserialize a CalculateSortedRank serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the CalculateSortedRank </param>
		''' <returns> CalculateSortedRank </returns>
		Public Overridable Function deserializeSortedRank(ByVal str As String) As CalculateSortedRank
			Return load(str, GetType(CalculateSortedRank))
		End Function

		''' <summary>
		''' Deserialize a SequenceSplit serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the SequenceSplit </param>
		''' <returns> SequenceSplit </returns>
		Public Overridable Function deserializeSequenceSplit(ByVal str As String) As SequenceSplit
			Return load(str, GetType(SequenceSplit))
		End Function

		''' <summary>
		''' Deserialize a DataAction serialized using <seealso cref="serialize(Object)"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the DataAction </param>
		''' <returns> DataAction </returns>
		Public Overridable Function deserializeDataAction(ByVal str As String) As DataAction
			Return load(str, GetType(DataAction))
		End Function

		''' <summary>
		''' Deserialize a Transform List serialized using <seealso cref="serializeTransformList(List)"/>, or
		''' an array serialized using <seealso cref="serialize(Transform[])"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the Transform list </param>
		''' <returns> {@code List<Transform>} </returns>
		Public Overridable Function deserializeTransformList(ByVal str As String) As IList(Of Transform)
			Return load(str, GetType(ListWrappers.TransformList)).getList()
		End Function

		''' <summary>
		''' Deserialize a Filter List serialized using <seealso cref="serializeFilterList(List)"/>, or
		''' an array serialized using <seealso cref="serialize(Filter[])"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the Filter list </param>
		''' <returns> {@code List<Filter>} </returns>
		Public Overridable Function deserializeFilterList(ByVal str As String) As IList(Of Filter)
			Return load(str, GetType(ListWrappers.FilterList)).getList()
		End Function

		''' <summary>
		''' Deserialize a Condition List serialized using <seealso cref="serializeConditionList(List)"/>, or
		''' an array serialized using <seealso cref="serialize(Condition[])"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the Condition list </param>
		''' <returns> {@code List<Condition>} </returns>
		Public Overridable Function deserializeConditionList(ByVal str As String) As IList(Of Condition)
			Return load(str, GetType(ListWrappers.ConditionList)).getList()
		End Function

		''' <summary>
		''' Deserialize an IStringReducer List serialized using <seealso cref="serializeReducerList(List)"/>, or
		''' an array serialized using <seealso cref="serialize(IReducer[])"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the IStringReducer list </param>
		''' <returns> {@code List<IStringReducer>} </returns>
		Public Overridable Function deserializeReducerList(ByVal str As String) As IList(Of IAssociativeReducer)
			Return load(str, GetType(ListWrappers.ReducerList)).getList()
		End Function

		''' <summary>
		''' Deserialize a SequenceComparator List serialized using <seealso cref="serializeSequenceComparatorList(List)"/>, or
		''' an array serialized using <seealso cref="serialize(SequenceComparator[])"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the SequenceComparator list </param>
		''' <returns> {@code List<SequenceComparator>} </returns>
		Public Overridable Function deserializeSequenceComparatorList(ByVal str As String) As IList(Of SequenceComparator)
			Return load(str, GetType(ListWrappers.SequenceComparatorList)).getList()
		End Function

		''' <summary>
		''' Deserialize a DataAction List serialized using <seealso cref="serializeDataActionList(List)"/>, or
		''' an array serialized using <seealso cref="serialize(DataAction[])"/>
		''' </summary>
		''' <param name="str"> String representation (YAML/JSON) of the DataAction list </param>
		''' <returns> {@code List<DataAction>} </returns>
		Public Overridable Function deserializeDataActionList(ByVal str As String) As IList(Of DataAction)
			Return load(str, GetType(ListWrappers.DataActionList)).getList()
		End Function
	End Class

End Namespace