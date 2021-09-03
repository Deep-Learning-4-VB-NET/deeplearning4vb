Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.deeplearning4j.nn.weights.embeddings

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties("nonSerializableInit") @EqualsAndHashCode public class WeightInitEmbedding implements org.deeplearning4j.nn.weights.IWeightInit
	<Serializable>
	Public Class WeightInitEmbedding
		Implements IWeightInit

		Private serializableInit As EmbeddingInitializer
		Private nonSerializableInit As EmbeddingInitializer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WeightInitEmbedding(@NonNull EmbeddingInitializer embeddingInitializer)
		Public Sub New(ByVal embeddingInitializer As EmbeddingInitializer)
			Me.New((If(embeddingInitializer.jsonSerializable(), embeddingInitializer, Nothing)), (If(embeddingInitializer.jsonSerializable(), Nothing, embeddingInitializer)))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected WeightInitEmbedding(@JsonProperty("serializableInit") EmbeddingInitializer serializableInit, @JsonProperty("nonSerializableInit") EmbeddingInitializer nonSerializableInit)
		Protected Friend Sub New(ByVal serializableInit As EmbeddingInitializer, ByVal nonSerializableInit As EmbeddingInitializer)
			Me.serializableInit = serializableInit
			Me.nonSerializableInit = nonSerializableInit
		End Sub

		Public Overridable Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray Implements IWeightInit.init
'JAVA TO VB CONVERTER NOTE: The local variable init was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim init_Conflict As EmbeddingInitializer = If(serializableInit IsNot Nothing, serializableInit, nonSerializableInit)
			If init_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot initialize embedding layer weights: no EmbeddingInitializer is available." & " This can occur if you save network configuration, load it, and the try to ")
			End If

			Preconditions.checkState(shape(0) = init_Conflict.vocabSize(), "Parameters shape[0]=%s does not match embedding initializer vocab size of %s", shape(0), init_Conflict.vocabSize())
			Preconditions.checkState(shape(1) = init_Conflict.vectorSize(), "Parameters shape[1]=%s does not match embedding initializer vector size of %s", shape(1), init_Conflict.vectorSize())

			Dim reshaped As INDArray = paramView.reshape("c"c, shape)
			init_Conflict.loadWeightsInto(reshaped)

			'Now that we've loaded weights - let's clear the reference if it's non-serializable so it can be GC'd
			Me.nonSerializableInit = Nothing

			Return reshaped
		End Function

		Public Overridable Function shape() As Long()
			If serializableInit IsNot Nothing Then
				Return New Long(){serializableInit.vocabSize(), serializableInit.vectorSize()}
			ElseIf nonSerializableInit IsNot Nothing Then
				Return New Long(){nonSerializableInit.vocabSize(), nonSerializableInit.vectorSize()}
			End If
			Return Nothing
		End Function
	End Class

End Namespace