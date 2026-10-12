Imports System.IO
Imports NonNull = lombok.NonNull
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
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



	Public Class MinMaxSerializerStrategy
		Implements NormalizerSerializerStrategy(Of NormalizerMinMaxScaler)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void write(@NonNull NormalizerMinMaxScaler normalizer, @NonNull OutputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub write(ByVal normalizer As NormalizerMinMaxScaler, ByVal stream As Stream)
			Using dos As New DataOutputStream(stream)
				dos.writeBoolean(normalizer.isFitLabel())
				dos.writeDouble(normalizer.getTargetMin())
				dos.writeDouble(normalizer.getTargetMax())

				Nd4j.write(normalizer.getMin(), dos)
				Nd4j.write(normalizer.getMax(), dos)

				If normalizer.isFitLabel() Then
					Nd4j.write(normalizer.getLabelMin(), dos)
					Nd4j.write(normalizer.getLabelMax(), dos)
				End If
				dos.flush()
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler restore(@NonNull InputStream stream) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function restore(ByVal stream As Stream) As NormalizerMinMaxScaler
			Dim dis As New DataInputStream(stream)

			Dim fitLabels As Boolean = dis.readBoolean()
			Dim targetMin As Double = dis.readDouble()
			Dim targetMax As Double = dis.readDouble()

			Dim result As New NormalizerMinMaxScaler(targetMin, targetMax)
			result.fitLabel(fitLabels)
			result.setFeatureStats(Nd4j.read(dis), Nd4j.read(dis))
			If fitLabels Then
				result.setLabelStats(Nd4j.read(dis), Nd4j.read(dis))
			End If

			Return result
		End Function

		Public Overridable ReadOnly Property SupportedType As NormalizerType Implements NormalizerSerializerStrategy(Of NormalizerMinMaxScaler).getSupportedType
			Get
				Return NormalizerType.MIN_MAX
			End Get
		End Property
	End Class

End Namespace