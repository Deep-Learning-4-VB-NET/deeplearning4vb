Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException

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

Namespace org.deeplearning4j.nn.modelimport.keras.config

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasLayerConfigurationFactory
	Public Class KerasLayerConfigurationFactory

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static KerasLayerConfiguration get(System.Nullable<Integer> kerasMajorVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function get(ByVal kerasMajorVersion As Integer?) As KerasLayerConfiguration
			If kerasMajorVersion <> 1 AndAlso kerasMajorVersion <> 2 Then
				Throw New UnsupportedKerasConfigurationException("Keras major version has to be either 1 or 2 (" & kerasMajorVersion & " provided)")
			ElseIf kerasMajorVersion = 1 Then
				Return New Keras1LayerConfiguration()
			Else
				Return New Keras2LayerConfiguration()
			End If
		End Function
	End Class

End Namespace