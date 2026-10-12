Imports System
Imports System.IO
Imports org.nd4j.linalg.dataset.api.preprocessor

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

	Public MustInherit Class CustomSerializerStrategy(Of T As org.nd4j.linalg.dataset.api.preprocessor.Normalizer)
		Implements NormalizerSerializerStrategy(Of T)

		Public MustOverride Function restore(ByVal stream As Stream) As Normalizer
		Public MustOverride Sub write(ByVal normalizer As T, ByVal stream As Stream)
		Public Overridable ReadOnly Property SupportedType As NormalizerType Implements NormalizerSerializerStrategy(Of T).getSupportedType
			Get
				Return NormalizerType.CUSTOM
			End Get
		End Property

		''' <summary>
		''' Get the class of the supported custom serializer
		''' </summary>
		''' <returns> the class </returns>
		Public MustOverride ReadOnly Property SupportedClass As Type(Of T)
	End Class

End Namespace