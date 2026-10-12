Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.pooling

	Public Class KerasPoolingUtils

		''' <summary>
		''' Map Keras pooling layers to DL4J pooling types.
		''' </summary>
		''' <param name="className"> name of the Keras pooling class </param>
		''' <returns> DL4J pooling type </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.layers.PoolingType mapPoolingType(String className, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function mapPoolingType(ByVal className As String, ByVal conf As KerasLayerConfiguration) As PoolingType
			Dim poolingType As PoolingType
			If className.Equals(conf.getLAYER_CLASS_NAME_MAX_POOLING_2D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_MAX_POOLING_1D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_MAX_POOLING_3D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_1D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_2D()) Then
				poolingType = PoolingType.MAX
			ElseIf className.Equals(conf.getLAYER_CLASS_NAME_AVERAGE_POOLING_2D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_AVERAGE_POOLING_1D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_AVERAGE_POOLING_3D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_1D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_2D()) Then
				poolingType = PoolingType.AVG
			Else
				Throw New UnsupportedKerasConfigurationException("Unsupported Keras pooling layer " & className)
			End If
			Return poolingType
		End Function

		''' <summary>
		''' Map Keras pooling layers to DL4J pooling dimensions.
		''' </summary>
		''' <param name="className"> name of the Keras pooling class </param>
		''' <param name="dimOrder"> the dimension order to determine which pooling dimensions to use </param>
		''' <returns> pooling dimensions as int array </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int[] mapGlobalPoolingDimensions(String className, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, org.deeplearning4j.nn.modelimport.keras.KerasLayer.DimOrder dimOrder) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function mapGlobalPoolingDimensions(ByVal className As String, ByVal conf As KerasLayerConfiguration, ByVal dimOrder As KerasLayer.DimOrder) As Integer()
			Dim dimensions() As Integer = Nothing
			If className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_1D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_1D()) Then
				Select Case dimOrder
					Case KerasLayer.DimOrder.THEANO
						dimensions = New Integer(){2}
					Case Else
						dimensions = New Integer(){1}
				End Select
			ElseIf className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_2D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_2D()) Then
				Select Case dimOrder
					Case KerasLayer.DimOrder.THEANO
						dimensions = New Integer(){2, 3}
					Case Else
						dimensions = New Integer(){1, 2}
				End Select
			ElseIf className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_3D()) OrElse className.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_3D()) Then
				Select Case dimOrder
					Case KerasLayer.DimOrder.THEANO
						dimensions = New Integer(){2, 3, 4}
					Case Else
						dimensions = New Integer(){1, 2, 3}
				End Select
			Else
				Throw New UnsupportedKerasConfigurationException("Unsupported Keras pooling layer " & className)
			End If

			Return dimensions
		End Function
	End Class

End Namespace