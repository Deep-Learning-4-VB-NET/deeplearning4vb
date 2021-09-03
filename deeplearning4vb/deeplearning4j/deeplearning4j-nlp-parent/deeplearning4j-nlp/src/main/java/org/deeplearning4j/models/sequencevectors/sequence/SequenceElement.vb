Imports System
Imports System.Collections.Generic
Imports AtomicDouble = org.nd4j.shade.guava.util.concurrent.AtomicDouble
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports HashUtil = org.nd4j.linalg.util.HashUtil
Imports org.nd4j.shade.jackson.annotation
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

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

Namespace org.deeplearning4j.models.sequencevectors.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonSubTypes({ @JsonSubTypes.Type(value = VocabWord.class, name = "vocabWord") }) @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) public abstract class SequenceElement implements Comparable<SequenceElement>, java.io.Serializable
	<Serializable>
	Public MustInherit Class SequenceElement
		Implements IComparable(Of SequenceElement)

		Private Const serialVersionUID As Long = 2223750736522624732L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty protected org.nd4j.shade.guava.util.concurrent.AtomicDouble elementFrequency = new org.nd4j.shade.guava.util.concurrent.AtomicDouble(0);
'JAVA TO VB CONVERTER NOTE: The field elementFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend elementFrequency_Conflict As New AtomicDouble(0)

		'used in comparison when building the huffman tree
'JAVA TO VB CONVERTER NOTE: The field index was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend index_Conflict As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field codes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend codes_Conflict As IList(Of SByte) = New List(Of SByte)()

'JAVA TO VB CONVERTER NOTE: The field points was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend points_Conflict As IList(Of Integer) = New List(Of Integer)()
'JAVA TO VB CONVERTER NOTE: The field codeLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend codeLength_Conflict As Short = 0

		' this var defines, if this token can't be truncated with minWordFrequency threshold
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean special;
		Protected Friend special As Boolean

		' this var defines that we have label here
		Protected Friend isLabel As Boolean

		' this var defines how many documents/sequences contain this word
'JAVA TO VB CONVERTER NOTE: The field sequencesCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend sequencesCount_Conflict As New AtomicLong(0)


		' this var is used as state for preciseWeightInit routine, to avoid multiple initializations for the same data
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean init;
		Protected Friend init As Boolean

	'    
	'            Reserved for Joint/Distributed vocabs mechanics
	'    
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected System.Nullable<Long> storageId;
		Protected Friend storageId As Long?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean isLocked;
		Protected Friend isLocked As Boolean

		''' <summary>
		''' This method should return string representation of this SequenceElement, so it can be used for
		''' 
		''' @return
		''' </summary>
		Public MustOverride ReadOnly Property Label As String

		''' <summary>
		''' This method returns number of documents/sequences where this element was evidenced
		''' 
		''' @return
		''' </summary>
		Public Overridable Property SequencesCount As Long
			Get
				Return sequencesCount_Conflict.get()
			End Get
			Set(ByVal count As Long)
				Me.sequencesCount_Conflict.set(count)
			End Set
		End Property


		''' <summary>
		''' Increments document count by one
		''' </summary>
		Public Overridable Sub incrementSequencesCount()
			Me.sequencesCount_Conflict.incrementAndGet()
		End Sub

		''' <summary>
		''' Increments document count by specified value </summary>
		''' <param name="count"> </param>
		Public Overridable Sub incrementSequencesCount(ByVal count As Long)
			Me.sequencesCount_Conflict.addAndGet(count)
		End Sub

		''' <summary>
		''' Returns whether this element was defined as label, or no
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Label As Boolean
			Get
				Return isLabel
			End Get
		End Property

		''' <summary>
		''' This method specifies, whether this element should be treated as label for some sequence/document or not.
		''' </summary>
		''' <param name="isLabel"> </param>
		Public Overridable Sub markAsLabel(ByVal isLabel As Boolean)
			Me.isLabel = isLabel
		End Sub

		''' <summary>
		''' This method returns SequenceElement's frequency in current training corpus.
		''' 
		''' @return
		''' </summary>
		Public Overridable Property ElementFrequency As Double
			Get
				Return elementFrequency_Conflict.get()
			End Get
			Set(ByVal value As Long)
				elementFrequency_Conflict.set(value)
			End Set
		End Property


		''' <summary>
		''' Increases element frequency counter by 1
		''' </summary>
		Public Overridable Sub incrementElementFrequency()
			increaseElementFrequency(1)
		End Sub

		''' <summary>
		''' Increases element frequency counter by argument
		''' </summary>
		''' <param name="by"> </param>
		Public Overridable Sub increaseElementFrequency(ByVal by As Integer)
			elementFrequency_Conflict.getAndAdd(by)
		End Sub

		''' <summary>
		''' Equals method override should be properly implemented for any extended class, otherwise it will be based on label equality
		''' </summary>
		''' <param name="object">
		''' @return </param>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			If Me Is [object] Then
				Return True
			End If
			If [object] Is Nothing Then
				Return False
			End If
			If Not (TypeOf [object] Is SequenceElement) Then
				Return False
			End If

			Return Me.Label.Equals(DirectCast([object], SequenceElement).Label)
		End Function

		''' <summary>
		'''  Returns index in Huffman tree
		''' </summary>
		''' <returns> index >= 0, if tree was built, -1 otherwise </returns>
		Public Overridable Property Index As Integer
			Get
				Return index_Conflict
			End Get
			Set(ByVal index As Integer)
				Me.index_Conflict = index
			End Set
		End Property


		''' <summary>
		''' Returns Huffman tree codes
		''' @return
		''' </summary>
		Public Overridable Property Codes As IList(Of SByte)
			Get
				Return codes_Conflict
			End Get
			Set(ByVal codes As IList(Of SByte))
				Me.codes_Conflict = codes
			End Set
		End Property


		''' <summary>
		''' Returns Huffman tree points
		''' 
		''' @return
		''' </summary>
		Public Overridable Property Points As IList(Of Integer)
			Get
				Return points_Conflict
			End Get
			Set(ByVal points As IList(Of Integer))
				Me.points_Conflict = points
			End Set
		End Property


		''' <summary>
		''' Sets Huffman tree points
		''' </summary>
		''' <param name="points"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public void setPoints(int[] points)
		Public Overridable WriteOnly Property Points As Integer()
			Set(ByVal points() As Integer)
				Me.points_Conflict = New List(Of Integer)()
				For i As Integer = 0 To points.Length - 1
					Me.points_Conflict.Add(points(i))
				Next i
			End Set
		End Property

		''' <summary>
		''' Returns Huffman code length.
		''' 
		''' Please note: maximum vocabulary/tree size depends on code length
		''' 
		''' @return
		''' </summary>
		Public Overridable Property CodeLength As Integer
			Get
				Return codeLength_Conflict
			End Get
			Set(ByVal codeLength As Short)
				Me.codeLength_Conflict = codeLength
				If codes_Conflict.Count < codeLength Then
					For i As Integer = 0 To codeLength - 1
						codes_Conflict.Add(CSByte(0))
					Next i
				End If
    
				If points_Conflict.Count < codeLength Then
					For i As Integer = 0 To codeLength - 1
						points_Conflict.Add(0)
					Next i
				End If
			End Set
		End Property



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static final long getLongHash(@NonNull String string)
		Public Shared Function getLongHash(ByVal [string] As String) As Long
			Return HashUtil.getLongHash([string])
		End Function

		''' <summary>
		''' Returns gradient for this specific element, at specific position </summary>
		''' <param name="index"> </param>
		''' <param name="g"> </param>
		''' <param name="lr">
		''' @return </param>
		<Obsolete>
		Public Overridable Function getGradient(ByVal index As Integer, ByVal g As Double, ByVal lr As Double) As Double
	'        
	'        if (adaGrad == null)
	'            adaGrad = new AdaGrad(1,getCodeLength(), lr);
	'        
	'        return adaGrad.getGradient(g, index, new int[]{1, getCodeLength()});
	'        
			Return 0.0
		End Function

		<Obsolete>
		Public Overridable Property HistoricalGradient As INDArray
			Set(ByVal gradient As INDArray)
		'        
		'        if (adaGrad == null)
		'            adaGrad = new AdaGrad(1,getCodeLength(), 0.025);
		'        
		'        adaGrad.setHistoricalGradient(gradient);
		'        
			End Set
			Get
		'        
		'        if (adaGrad == null)
		'            adaGrad = new AdaGrad(1,getCodeLength(), 0.025);
		'        return adaGrad.getHistoricalGradient();
		'        
				Return Nothing
			End Get
		End Property


		''' <summary>
		''' hashCode method override should be properly implemented for any extended class, otherwise it will be based on label hashCode
		''' </summary>
		''' <returns> hashCode for this SequenceElement </returns>
		Public Overrides Function GetHashCode() As Integer
			If Me.Label Is Nothing Then
				Throw New System.InvalidOperationException("Label should not be null")
			End If
			Return Me.Label.GetHashCode()
		End Function

		Public Overridable Function CompareTo(ByVal o As SequenceElement) As Integer Implements IComparable(Of SequenceElement).CompareTo
			Return elementFrequency_Conflict.get().CompareTo(o.elementFrequency_Conflict.get())
		End Function

		Public Overrides Function ToString() As String
			Return "SequenceElement: {label: '" & Me.Label & "'," & " freq: '" & elementFrequency_Conflict.get() & "'," & " codes: " & codes_Conflict.ToString() & " points: " & points_Conflict.ToString() & " index: '" & Me.index_Conflict & "'}"
		End Function

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public MustOverride Function toJSON() As String

		Public Overridable ReadOnly Property StorageId As Long?
			Get
				If storageId Is Nothing Then
					storageId = SequenceElement.getLongHash(Me.Label)
				End If
				Return storageId
			End Get
		End Property

		Public Shared Function mapper() As ObjectMapper
	'        
	'              DO NOT ENABLE INDENT_OUTPUT FEATURE
	'              we need THIS json to be single-line
	'          
			Dim ret As New ObjectMapper()
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			Return ret
		End Function
	End Class

End Namespace