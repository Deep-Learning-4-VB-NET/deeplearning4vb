Imports System.IO
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler

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


	Public Class ImagePreProcessingSerializerStrategy
		Implements NormalizerSerializerStrategy(Of ImagePreProcessingScaler)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler normalizer, OutputStream stream) throws IOException
		Public Overridable Sub write(ByVal normalizer As ImagePreProcessingScaler, ByVal stream As Stream)
			Using dataOutputStream As New DataOutputStream(stream)
				dataOutputStream.writeDouble(normalizer.getMinRange())
				dataOutputStream.writeDouble(normalizer.getMaxRange())
				dataOutputStream.writeDouble(normalizer.getMaxPixelVal())
				dataOutputStream.flush()
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler restore(InputStream stream) throws IOException
		Public Overridable Function restore(ByVal stream As Stream) As ImagePreProcessingScaler
			Dim dataOutputStream As New DataInputStream(stream)
			Dim minRange As Double = dataOutputStream.readDouble()
			Dim maxRange As Double = dataOutputStream.readDouble()
			Dim maxPixelVal As Double = dataOutputStream.readDouble()
			Dim ret As New ImagePreProcessingScaler(minRange,maxRange)
			ret.setMaxPixelVal(maxPixelVal)
			Return ret
		End Function

		Public Overridable ReadOnly Property SupportedType As NormalizerType Implements NormalizerSerializerStrategy(Of ImagePreProcessingScaler).getSupportedType
			Get
				Return NormalizerType.IMAGE_MIN_MAX
			End Get
		End Property
	End Class

End Namespace