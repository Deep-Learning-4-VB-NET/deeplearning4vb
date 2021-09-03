Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports MultiNormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerStandardize
Imports DistributionStats = org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.serializer


	Public Class MultiStandardizeSerializerStrategy
		Implements NormalizerSerializerStrategy(Of MultiNormalizerStandardize)

		''' <summary>
		''' Serialize a MultiNormalizerStandardize to a output stream
		''' </summary>
		''' <param name="normalizer"> the normalizer </param>
		''' <param name="stream">     the output stream to write to </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void write(@NonNull MultiNormalizerStandardize normalizer, @NonNull OutputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub write(ByVal normalizer As MultiNormalizerStandardize, ByVal stream As Stream)
			Using dos As New DataOutputStream(stream)
				dos.writeBoolean(normalizer.isFitLabel())
				dos.writeInt(normalizer.numInputs())
				dos.writeInt(If(normalizer.isFitLabel(), normalizer.numOutputs(), -1))

				Dim i As Integer = 0
				Do While i < normalizer.numInputs()
					Nd4j.write(normalizer.getFeatureMean(i), dos)
					Nd4j.write(normalizer.getFeatureStd(i), dos)
					i += 1
				Loop
				If normalizer.isFitLabel() Then
					i = 0
					Do While i < normalizer.numOutputs()
						Nd4j.write(normalizer.getLabelMean(i), dos)
						Nd4j.write(normalizer.getLabelStd(i), dos)
						i += 1
					Loop
				End If
				dos.flush()
			End Using
		End Sub

		''' <summary>
		''' Restore a MultiNormalizerStandardize that was previously serialized by this strategy
		''' </summary>
		''' <param name="stream"> the input stream to restore from </param>
		''' <returns> the restored MultiNormalizerStandardize </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerStandardize restore(@NonNull InputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function restore(ByVal stream As Stream) As MultiNormalizerStandardize
			Dim dis As New DataInputStream(stream)
			Dim fitLabels As Boolean = dis.readBoolean()
			Dim numInputs As Integer = dis.readInt()
			Dim numOutputs As Integer = dis.readInt()

			Dim result As New MultiNormalizerStandardize()
			result.fitLabel(fitLabels)

			Dim featureStats As IList(Of DistributionStats) = New List(Of DistributionStats)()
			For i As Integer = 0 To numInputs - 1
				featureStats.Add(New DistributionStats(Nd4j.read(dis), Nd4j.read(dis)))
			Next i
			result.setFeatureStats(featureStats)

			If fitLabels Then
				Dim labelStats As IList(Of DistributionStats) = New List(Of DistributionStats)()
				For i As Integer = 0 To numOutputs - 1
					labelStats.Add(New DistributionStats(Nd4j.read(dis), Nd4j.read(dis)))
				Next i
				result.setLabelStats(labelStats)
			End If

			Return result
		End Function

		Public Overridable ReadOnly Property SupportedType As NormalizerType Implements NormalizerSerializerStrategy(Of MultiNormalizerStandardize).getSupportedType
			Get
				Return NormalizerType.MULTI_STANDARDIZE
			End Get
		End Property
	End Class

End Namespace