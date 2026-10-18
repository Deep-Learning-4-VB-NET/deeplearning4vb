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

Imports System
Imports System.Collections.Generic
Imports System.Text.Json
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports CalculateSortedRankT = DeepLearning4VB.org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Serialization of the parts of a transform process (transforms, filters, conditions, reducers, ...) and of lists
    ''' of them. Lists are written wrapped as <c>{"list" : [...]}</c> (see <see cref="ListWrappers"/>) so that their
    ''' elements can be read back. Only JSON is supported (<see cref="JsonSerializer"/>); the Java YAML serializer is not
    ''' ported. Errors surface as <see cref="JsonException"/> (or <see cref="NotSupportedException"/> for types
    ''' System.Text.Json cannot handle) where Java wrapped them in a <c>RuntimeException</c>.
    ''' </summary>
    Public MustInherit Class BaseSerializer

        ''' <summary>The options used to read and write JSON; created by <see cref="JsonMappers"/>.</summary>
        Public MustOverride ReadOnly Property ObjectMapper As JsonSerializerOptions

        ''' <summary>Java <c>getObjectMapper()</c>.</summary>
        Public Function getObjectMapper() As JsonSerializerOptions
            Return ObjectMapper
        End Function

        Private Function load(Of T)(ByVal str As String) As T
            Return DirectCast(JsonMappers.readValue(str, GetType(T), ObjectMapper), T)
        End Function

        Private Function write(ByVal value As Object, ByVal declaredType As Type) As String
            Return JsonMappers.writeValueAsString(value, declaredType, ObjectMapper)
        End Function

        ''' <summary>
        ''' Serialize the specified object, such as a <see cref="TransformT"/>, <see cref="ConditionT"/>, <see cref="FilterT"/>, etc<br/>
        ''' <b>NOTE:</b> For lists use the list methods, such as <see cref="serializeTransformList(IList(Of TransformT))"/><br/>
        ''' <para>
        ''' To deserialize, use the appropriate method: <see cref="deserializeTransform(String)"/> for example.
        ''' </para>
        ''' </summary>
        ''' <param name="o"> Object to serialize </param>
        ''' <returns> String (json) representation of the object </returns>
        Public Overridable Function serialize(ByVal o As Object) As String
            If o Is Nothing Then Return "null"
            Dim t As Type = o.GetType()
            Return write(o, If(JsonMappers.wrapperRootOf(t), t))
        End Function

        '===================================================================
        'Wrappers for arrays and lists

        Public Overridable Function serialize(ByVal transforms() As TransformT) As String
            Return serializeTransformList(New List(Of TransformT)(transforms))
        End Function

        ''' <summary>
        ''' Serialize a list of Transforms
        ''' </summary>
        Public Overridable Function serializeTransformList(ByVal list As IList(Of TransformT)) As String
            Return write(New ListWrappers.TransformList(list), GetType(ListWrappers.TransformList))
        End Function

        Public Overridable Function serialize(ByVal filters() As FilterT) As String
            Return serializeFilterList(New List(Of FilterT)(filters))
        End Function

        ''' <summary>
        ''' Serialize a list of Filters
        ''' </summary>
        Public Overridable Function serializeFilterList(ByVal list As IList(Of FilterT)) As String
            Return write(New ListWrappers.FilterList(list), GetType(ListWrappers.FilterList))
        End Function

        Public Overridable Function serialize(ByVal conditions() As ConditionT) As String
            Return serializeConditionList(New List(Of ConditionT)(conditions))
        End Function

        ''' <summary>
        ''' Serialize a list of Conditions
        ''' </summary>
        Public Overridable Function serializeConditionList(ByVal list As IList(Of ConditionT)) As String
            Return write(New ListWrappers.ConditionList(list), GetType(ListWrappers.ConditionList))
        End Function

        Public Overridable Function serialize(ByVal reducers() As IAssociativeReducerT) As String
            Return serializeReducerList(New List(Of IAssociativeReducerT)(reducers))
        End Function

        ''' <summary>
        ''' Serialize a list of IReducers
        ''' </summary>
        Public Overridable Function serializeReducerList(ByVal list As IList(Of IAssociativeReducerT)) As String
            Return write(New ListWrappers.ReducerList(list), GetType(ListWrappers.ReducerList))
        End Function

        Public Overridable Function serialize(ByVal seqComparators() As SequenceComparatorT) As String
            Return serializeSequenceComparatorList(New List(Of SequenceComparatorT)(seqComparators))
        End Function

        ''' <summary>
        ''' Serialize a list of SequenceComparators
        ''' </summary>
        Public Overridable Function serializeSequenceComparatorList(ByVal list As IList(Of SequenceComparatorT)) As String
            Return write(New ListWrappers.SequenceComparatorList(list), GetType(ListWrappers.SequenceComparatorList))
        End Function

        Public Overridable Function serialize(ByVal dataActions() As DataActionT) As String
            Return serializeDataActionList(New List(Of DataActionT)(dataActions))
        End Function

        ''' <summary>
        ''' Serialize a list of DataActions
        ''' </summary>
        Public Overridable Function serializeDataActionList(ByVal list As IList(Of DataActionT)) As String
            Return write(New ListWrappers.DataActionList(list), GetType(ListWrappers.DataActionList))
        End Function

        '======================================================================
        ' Deserialization methods

        ''' <summary>
        ''' Deserialize a Transform serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the Transform </param>
        ''' <returns> Transform </returns>
        Public Overridable Function deserializeTransform(ByVal str As String) As TransformT
            Return load(Of TransformT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a Filter serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the Filter </param>
        ''' <returns> Filter </returns>
        Public Overridable Function deserializeFilter(ByVal str As String) As FilterT
            Return load(Of FilterT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a Condition serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the Condition </param>
        ''' <returns> Condition </returns>
        Public Overridable Function deserializeCondition(ByVal str As String) As ConditionT
            Return load(Of ConditionT)(str)
        End Function

        ''' <summary>
        ''' Deserialize an IAssociativeReducer serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the IAssociativeReducer </param>
        ''' <returns> IAssociativeReducer </returns>
        Public Overridable Function deserializeReducer(ByVal str As String) As IAssociativeReducerT
            Return load(Of IAssociativeReducerT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a SequenceComparator serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the SequenceComparator </param>
        ''' <returns> SequenceComparator </returns>
        Public Overridable Function deserializeSequenceComparator(ByVal str As String) As SequenceComparatorT
            Return load(Of SequenceComparatorT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a CalculateSortedRank serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the CalculateSortedRank </param>
        ''' <returns> CalculateSortedRank </returns>
        Public Overridable Function deserializeSortedRank(ByVal str As String) As CalculateSortedRankT
            Return load(Of CalculateSortedRankT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a SequenceSplit serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the SequenceSplit </param>
        ''' <returns> SequenceSplit </returns>
        Public Overridable Function deserializeSequenceSplit(ByVal str As String) As SequenceSplitT
            Return load(Of SequenceSplitT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a DataAction serialized using <see cref="serialize(Object)"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the DataAction </param>
        ''' <returns> DataAction </returns>
        Public Overridable Function deserializeDataAction(ByVal str As String) As DataActionT
            Return load(Of DataActionT)(str)
        End Function

        ''' <summary>
        ''' Deserialize a Transform List serialized using <see cref="serializeTransformList(IList(Of TransformT))"/>, or
        ''' an array serialized using <see cref="serialize(TransformT())"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the Transform list </param>
        ''' <returns> <c>List(Of Transform)</c> </returns>
        Public Overridable Function deserializeTransformList(ByVal str As String) As IList(Of TransformT)
            Return load(Of ListWrappers.TransformList)(str).getList()
        End Function

        ''' <summary>
        ''' Deserialize a Filter List serialized using <see cref="serializeFilterList(IList(Of FilterT))"/>, or
        ''' an array serialized using <see cref="serialize(FilterT())"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the Filter list </param>
        ''' <returns> <c>List(Of Filter)</c> </returns>
        Public Overridable Function deserializeFilterList(ByVal str As String) As IList(Of FilterT)
            Return load(Of ListWrappers.FilterList)(str).getList()
        End Function

        ''' <summary>
        ''' Deserialize a Condition List serialized using <see cref="serializeConditionList(IList(Of ConditionT))"/>, or
        ''' an array serialized using <see cref="serialize(ConditionT())"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the Condition list </param>
        ''' <returns> <c>List(Of Condition)</c> </returns>
        Public Overridable Function deserializeConditionList(ByVal str As String) As IList(Of ConditionT)
            Return load(Of ListWrappers.ConditionList)(str).getList()
        End Function

        ''' <summary>
        ''' Deserialize an IAssociativeReducer List serialized using <see cref="serializeReducerList(IList(Of IAssociativeReducerT))"/>, or
        ''' an array serialized using <see cref="serialize(IAssociativeReducerT())"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the IAssociativeReducer list </param>
        ''' <returns> <c>List(Of IAssociativeReducer)</c> </returns>
        Public Overridable Function deserializeReducerList(ByVal str As String) As IList(Of IAssociativeReducerT)
            Return load(Of ListWrappers.ReducerList)(str).getList()
        End Function

        ''' <summary>
        ''' Deserialize a SequenceComparator List serialized using <see cref="serializeSequenceComparatorList(IList(Of SequenceComparatorT))"/>, or
        ''' an array serialized using <see cref="serialize(SequenceComparatorT())"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the SequenceComparator list </param>
        ''' <returns> <c>List(Of SequenceComparator)</c> </returns>
        Public Overridable Function deserializeSequenceComparatorList(ByVal str As String) As IList(Of SequenceComparatorT)
            Return load(Of ListWrappers.SequenceComparatorList)(str).getList()
        End Function

        ''' <summary>
        ''' Deserialize a DataAction List serialized using <see cref="serializeDataActionList(IList(Of DataActionT))"/>, or
        ''' an array serialized using <see cref="serialize(DataActionT())"/>
        ''' </summary>
        ''' <param name="str"> String representation (JSON) of the DataAction list </param>
        ''' <returns> <c>List(Of DataAction)</c> </returns>
        Public Overridable Function deserializeDataActionList(ByVal str As String) As IList(Of DataActionT)
            Return load(Of ListWrappers.DataActionList)(str).getList()
        End Function

    End Class

End Namespace
