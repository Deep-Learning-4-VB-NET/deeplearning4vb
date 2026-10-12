Imports System
Imports System.Collections.Generic
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolutional


	Public Class KerasConvolutionUtils




		''' <summary>
		''' Get (convolution) stride from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Strides array from Keras configuration </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int[] getStrideFromConfig(java.util.Map<String, Object> layerConfig, int dimension, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getStrideFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal dimension As Integer, ByVal conf As KerasLayerConfiguration) As Integer()
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim strides() As Integer
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_CONVOLUTION_STRIDES()) AndAlso dimension >= 2 Then
				' 2D/3D Convolutional layers. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> stridesList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_CONVOLUTION_STRIDES());
				Dim stridesList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_CONVOLUTION_STRIDES()), IList(Of Integer))
				strides = ArrayUtil.toArray(stridesList)
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_SUBSAMPLE_LENGTH()) AndAlso dimension = 1 Then
				' 1D Convolutional layers. 
				If DirectCast(layerConfig("keras_version"), Integer) = 2 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> stridesList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_SUBSAMPLE_LENGTH());
					Dim stridesList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_SUBSAMPLE_LENGTH()), IList(Of Integer))
					strides = ArrayUtil.toArray(stridesList)
				Else
					Dim subsampleLength As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_SUBSAMPLE_LENGTH()), Integer)
					strides = New Integer(){subsampleLength}
				End If
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_POOL_STRIDES()) AndAlso dimension >= 2 Then
				' 2D/3D Pooling layers. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> stridesList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_POOL_STRIDES());
				Dim stridesList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_POOL_STRIDES()), IList(Of Integer))
				strides = ArrayUtil.toArray(stridesList)
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_POOL_1D_STRIDES()) AndAlso dimension = 1 Then
				' 1D Pooling layers. 
				Dim stride As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_POOL_1D_STRIDES()), Integer)
				strides = New Integer(){stride}
			Else
				Throw New InvalidKerasConfigurationException("Could not determine layer stride: no " & conf.getLAYER_FIELD_CONVOLUTION_STRIDES() & " or " & conf.getLAYER_FIELD_POOL_STRIDES() & " field found")
			End If
			Return strides
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static int getDepthMultiplier(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Friend Shared Function getDepthMultiplier(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Integer
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Return DirectCast(innerConfig(conf.getLAYER_FIELD_DEPTH_MULTIPLIER()), Integer)
		End Function

		''' <summary>
		''' Get atrous / dilation rate from config
		''' </summary>
		''' <param name="layerConfig">   dictionary containing Keras layer configuration </param>
		''' <param name="dimension">     dimension of the convolution layer (1 or 2) </param>
		''' <param name="conf">          Keras layer configuration </param>
		''' <param name="forceDilation"> boolean to indicate if dilation argument should be in config </param>
		''' <returns> list of integers with atrous rates
		''' </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int[] getDilationRate(java.util.Map<String, Object> layerConfig, int dimension, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, boolean forceDilation) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getDilationRate(ByVal layerConfig As IDictionary(Of String, Object), ByVal dimension As Integer, ByVal conf As KerasLayerConfiguration, ByVal forceDilation As Boolean) As Integer()
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim atrousRate() As Integer
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_DILATION_RATE()) AndAlso dimension >= 2 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> atrousRateList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_DILATION_RATE());
				Dim atrousRateList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_DILATION_RATE()), IList(Of Integer))
				atrousRate = ArrayUtil.toArray(atrousRateList)
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_DILATION_RATE()) AndAlso dimension = 1 Then
				If DirectCast(layerConfig("keras_version"), Integer) = 2 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> atrousRateList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_DILATION_RATE());
					Dim atrousRateList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_DILATION_RATE()), IList(Of Integer))
					atrousRate = New Integer(){atrousRateList(0), atrousRateList(0)}
				Else
					Dim atrous As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_DILATION_RATE()), Integer)
					atrousRate = New Integer(){atrous, atrous}
				End If
			Else
				' If we are using keras 1, for regular convolutions, there is no "atrous" argument, for keras
				' 2 there always is.
				If forceDilation Then
					Throw New InvalidKerasConfigurationException("Could not determine dilation rate: no " & conf.getLAYER_FIELD_DILATION_RATE() & " field found")
				Else
					atrousRate = Nothing
				End If
			End If
			Return atrousRate

		End Function


		''' <summary>
		''' Return the <seealso cref="Convolution3D.DataFormat"/>
		''' from the configuration .
		''' If the value is <seealso cref="KerasLayerConfiguration.getDIM_ORDERING_TENSORFLOW()"/>
		''' then the value is <seealso cref="Convolution3D.DataFormat.NDHWC "/>
		''' else it's <seealso cref="KerasLayerConfiguration.getDIM_ORDERING_THEANO()"/>
		''' which is <seealso cref="Convolution3D.DataFormat.NDHWC"/> </summary>
		''' <param name="layerConfig"> the layer configuration to get the values from </param>
		''' <param name="layerConfiguration"> the keras configuration used for retrieving
		'''                           values from the configuration </param>
		''' <returns> the <seealso cref="CNN2DFormat"/> given the configuration </returns>
		''' <exception cref="InvalidKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.layers.Convolution3D.DataFormat getCNN3DDataFormatFromConfig(java.util.Map<String,Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration layerConfiguration) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getCNN3DDataFormatFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal layerConfiguration As KerasLayerConfiguration) As Convolution3D.DataFormat
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig,layerConfiguration)
			Dim dataFormat As String = If(innerConfig.ContainsKey(layerConfiguration.getLAYER_FIELD_DIM_ORDERING()), innerConfig(layerConfiguration.getLAYER_FIELD_DIM_ORDERING()).ToString(), "channels_last")
			Return If(dataFormat.Equals("channels_last"), Convolution3D.DataFormat.NDHWC, Convolution3D.DataFormat.NCDHW)

		End Function

		''' <summary>
		''' Return the <seealso cref="CNN2DFormat"/>
		''' from the configuration .
		''' If the value is <seealso cref="KerasLayerConfiguration.getDIM_ORDERING_TENSORFLOW()"/>
		''' then the value is <seealso cref="CNN2DFormat.NHWC"/>
		''' else it's <seealso cref="KerasLayerConfiguration.getDIM_ORDERING_THEANO()"/>
		''' which is <seealso cref="CNN2DFormat.NCHW"/> </summary>
		''' <param name="layerConfig"> the layer configuration to get the values from </param>
		''' <param name="layerConfiguration"> the keras configuration used for retrieving
		'''                           values from the configuration </param>
		''' <returns> the <seealso cref="CNN2DFormat"/> given the configuration </returns>
		''' <exception cref="InvalidKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.CNN2DFormat getDataFormatFromConfig(java.util.Map<String,Object> layerConfig,org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration layerConfiguration) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getDataFormatFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal layerConfiguration As KerasLayerConfiguration) As CNN2DFormat
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig,layerConfiguration)
			Dim dataFormat As String = If(innerConfig.ContainsKey(layerConfiguration.getLAYER_FIELD_DIM_ORDERING()), innerConfig(layerConfiguration.getLAYER_FIELD_DIM_ORDERING()).ToString(), "channels_last")
			Return If(dataFormat.Equals("channels_last"), CNN2DFormat.NHWC, CNN2DFormat.NCHW)

		End Function

		''' <summary>
		''' Get upsampling size from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration
		''' </param>
		''' <returns> Upsampling integer array from Keras config </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static int[] getUpsamplingSizeFromConfig(java.util.Map<String, Object> layerConfig, int dimension, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Friend Shared Function getUpsamplingSizeFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal dimension As Integer, ByVal conf As KerasLayerConfiguration) As Integer()
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim size() As Integer
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_UPSAMPLING_2D_SIZE()) AndAlso dimension = 2 OrElse innerConfig.ContainsKey(conf.getLAYER_FIELD_UPSAMPLING_3D_SIZE()) AndAlso dimension = 3 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> sizeList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_UPSAMPLING_2D_SIZE());
				Dim sizeList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_UPSAMPLING_2D_SIZE()), IList(Of Integer))
				size = ArrayUtil.toArray(sizeList)
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_UPSAMPLING_1D_SIZE()) AndAlso dimension = 1 Then
				Dim upsamplingSize1D As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_UPSAMPLING_1D_SIZE()), Integer)
				size = New Integer(){upsamplingSize1D}
			Else
				Throw New InvalidKerasConfigurationException("Could not determine kernel size: no " & conf.getLAYER_FIELD_UPSAMPLING_1D_SIZE() & ", " & conf.getLAYER_FIELD_UPSAMPLING_2D_SIZE())
			End If
			Return size
		End Function


		''' <summary>
		''' Get (convolution) kernel size from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration
		''' </param>
		''' <returns> Convolutional kernel sizes </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int[] getKernelSizeFromConfig(java.util.Map<String, Object> layerConfig, int dimension, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, int kerasMajorVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getKernelSizeFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal dimension As Integer, ByVal conf As KerasLayerConfiguration, ByVal kerasMajorVersion As Integer) As Integer()
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim kernelSize() As Integer
			If kerasMajorVersion <> 2 Then
				If innerConfig.ContainsKey(conf.getLAYER_FIELD_NB_ROW()) AndAlso dimension = 2 AndAlso innerConfig.ContainsKey(conf.getLAYER_FIELD_NB_COL()) Then
					' 2D Convolutional layers. 
					Dim kernelSizeList As IList(Of Integer) = New List(Of Integer)()
					kernelSizeList.Add(DirectCast(innerConfig(conf.getLAYER_FIELD_NB_ROW()), Integer?))
					kernelSizeList.Add(DirectCast(innerConfig(conf.getLAYER_FIELD_NB_COL()), Integer?))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_3D_KERNEL_1()) AndAlso dimension = 3 AndAlso innerConfig.ContainsKey(conf.getLAYER_FIELD_3D_KERNEL_2()) AndAlso innerConfig.ContainsKey(conf.getLAYER_FIELD_3D_KERNEL_3()) Then
					' 3D Convolutional layers. 
					Dim kernelSizeList As IList(Of Integer) = New List(Of Integer)()
					kernelSizeList.Add(DirectCast(innerConfig(conf.getLAYER_FIELD_3D_KERNEL_1()), Integer?))
					kernelSizeList.Add(DirectCast(innerConfig(conf.getLAYER_FIELD_3D_KERNEL_2()), Integer?))
					kernelSizeList.Add(DirectCast(innerConfig(conf.getLAYER_FIELD_3D_KERNEL_3()), Integer?))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_FILTER_LENGTH()) AndAlso dimension = 1 Then
					' 1D Convolutional layers. 
					Dim filterLength As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_FILTER_LENGTH()), Integer)
					kernelSize = New Integer(){filterLength}
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_POOL_SIZE()) AndAlso dimension >= 2 Then
					' 2D/3D Pooling layers. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> kernelSizeList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_POOL_SIZE());
					Dim kernelSizeList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_POOL_SIZE()), IList(Of Integer))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_POOL_1D_SIZE()) AndAlso dimension = 1 Then
					' 1D Pooling layers. 
					Dim poolSize1D As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_POOL_1D_SIZE()), Integer)
					kernelSize = New Integer(){poolSize1D}
				Else
					Throw New InvalidKerasConfigurationException("Could not determine kernel size: no " & conf.getLAYER_FIELD_NB_ROW() & ", " & conf.getLAYER_FIELD_NB_COL() & ", or " & conf.getLAYER_FIELD_FILTER_LENGTH() & ", or " & conf.getLAYER_FIELD_POOL_1D_SIZE() & ", or " & conf.getLAYER_FIELD_POOL_SIZE() & " field found")
				End If
			Else
				' 2D/3D Convolutional layers. 
				If innerConfig.ContainsKey(conf.getLAYER_FIELD_KERNEL_SIZE()) AndAlso dimension >= 2 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> kernelSizeList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_KERNEL_SIZE());
					Dim kernelSizeList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_KERNEL_SIZE()), IList(Of Integer))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_FILTER_LENGTH()) AndAlso dimension = 1 Then
					' 1D Convolutional layers. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> kernelSizeList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_FILTER_LENGTH());
					Dim kernelSizeList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_FILTER_LENGTH()), IList(Of Integer))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_POOL_SIZE()) AndAlso dimension >= 2 Then
					' 2D Pooling layers. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> kernelSizeList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_POOL_SIZE());
					Dim kernelSizeList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_POOL_SIZE()), IList(Of Integer))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_POOL_1D_SIZE()) AndAlso dimension = 1 Then
					' 1D Pooling layers. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> kernelSizeList = (java.util.List<Integer>) innerConfig.get(conf.getLAYER_FIELD_POOL_1D_SIZE());
					Dim kernelSizeList As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_POOL_1D_SIZE()), IList(Of Integer))
					kernelSize = ArrayUtil.toArray(kernelSizeList)
				Else
					Throw New InvalidKerasConfigurationException("Could not determine kernel size: no " & conf.getLAYER_FIELD_KERNEL_SIZE() & ", or " & conf.getLAYER_FIELD_FILTER_LENGTH() & ", or " & conf.getLAYER_FIELD_POOL_SIZE() & " field found")
				End If
			End If

			Return kernelSize
		End Function

		''' <summary>
		''' Get convolution border mode from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Border mode of convolutional layers </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.ConvolutionMode getConvolutionModeFromConfig(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getConvolutionModeFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As ConvolutionMode
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_BORDER_MODE()) Then
				Throw New InvalidKerasConfigurationException("Could not determine convolution border mode: no " & conf.getLAYER_FIELD_BORDER_MODE() & " field found")
			End If
			Dim borderMode As String = DirectCast(innerConfig(conf.getLAYER_FIELD_BORDER_MODE()), String)
			Dim convolutionMode As ConvolutionMode
			If borderMode.Equals(conf.getLAYER_BORDER_MODE_SAME()) Then
	'             Keras relies upon the Theano and TensorFlow border mode definitions and operations:
	'             * TH: http://deeplearning.net/software/theano/library/tensor/nnet/conv.html#theano.tensor.nnet.conv.conv2d
	'             * TF: https://www.tensorflow.org/api_docs/python/nn/convolution#conv2d
	'             
				convolutionMode = ConvolutionMode.Same

			ElseIf borderMode.Equals(conf.getLAYER_BORDER_MODE_VALID()) OrElse borderMode.Equals(conf.getLAYER_BORDER_MODE_FULL()) Then
				convolutionMode = ConvolutionMode.Truncate
			ElseIf borderMode.Equals(conf.getLAYER_BORDER_MODE_CAUSAL()) Then
				convolutionMode = ConvolutionMode.Causal
			Else
				Throw New UnsupportedKerasConfigurationException("Unsupported convolution border mode: " & borderMode)
			End If
			Return convolutionMode
		End Function

		''' <summary>
		''' Get (convolution) padding from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Padding values derived from border mode </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int[] getPaddingFromBorderModeConfig(java.util.Map<String, Object> layerConfig, int dimension, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, int kerasMajorVersion) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getPaddingFromBorderModeConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal dimension As Integer, ByVal conf As KerasLayerConfiguration, ByVal kerasMajorVersion As Integer) As Integer()
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim padding() As Integer = Nothing
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_BORDER_MODE()) Then
				Throw New InvalidKerasConfigurationException("Could not determine convolution border mode: no " & conf.getLAYER_FIELD_BORDER_MODE() & " field found")
			End If
			Dim borderMode As String = DirectCast(innerConfig(conf.getLAYER_FIELD_BORDER_MODE()), String)
			If borderMode.Equals(conf.getLAYER_FIELD_BORDER_MODE()) Then
				padding = getKernelSizeFromConfig(layerConfig, dimension, conf, kerasMajorVersion)
				For i As Integer = 0 To padding.Length - 1
					padding(i) -= 1
				Next i
			End If
			Return padding
		End Function

		''' <summary>
		''' Get padding and cropping configurations from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <param name="conf">        KerasLayerConfiguration </param>
		''' <param name="layerField">  String value of the layer config name to check for (e.g. "padding" or "cropping") </param>
		''' <param name="dimension">   Dimension of the padding layer </param>
		''' <returns> padding list of integers </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static int[] getPaddingFromConfig(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, String layerField, int dimension) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Friend Shared Function getPaddingFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration, ByVal layerField As String, ByVal dimension As Integer) As Integer()
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(layerField) Then
				Throw New InvalidKerasConfigurationException("Field " & layerField & " not found in Keras cropping or padding layer")
			End If
			Dim padding() As Integer
			If dimension >= 2 Then
				Dim paddingList As IList(Of Integer)
				' For 2D layers, padding/cropping can either be a pair [[x_0, x_1].[y_0, y_1]] or a pair [x, y]
				' or a single integer x. Likewise for the 3D case.
				Try
					Dim paddingNoCast As System.Collections.IList = DirectCast(innerConfig(layerField), System.Collections.IList)
					Dim isNested As Boolean
					Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> firstItem = (java.util.List<Integer>) paddingNoCast.get(0);
						Dim firstItem As IList(Of Integer) = CType(paddingNoCast(0), IList(Of Integer))
						isNested = True
						paddingList = New List(Of Integer)(2 * dimension)
					Catch e As Exception
						Dim firstItem As Integer = CInt(paddingNoCast(0))
						isNested = False
						paddingList = New List(Of Integer)(dimension)
					End Try

					If (paddingNoCast.Count = dimension) AndAlso Not isNested Then
						For i As Integer = 0 To dimension - 1
							paddingList.Add(CInt(paddingNoCast(i)))
						Next i
						padding = ArrayUtil.toArray(paddingList)
					ElseIf (paddingNoCast.Count = dimension) AndAlso isNested Then
						For j As Integer = 0 To dimension - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<Integer> item = (java.util.List<Integer>) paddingNoCast.get(j);
							Dim item As IList(Of Integer) = CType(paddingNoCast(j), IList(Of Integer))
							paddingList.Add((item(0)))
							paddingList.Add((item(1)))
						Next j

						padding = ArrayUtil.toArray(paddingList)
					Else
						Throw New InvalidKerasConfigurationException("Found Keras ZeroPadding" & dimension & "D layer with invalid " & paddingList.Count & "D padding.")
					End If
				Catch e As Exception
					Dim paddingInt As Integer = DirectCast(innerConfig(layerField), Integer)
					If dimension = 2 Then
						padding = New Integer(){paddingInt, paddingInt, paddingInt, paddingInt}
					Else
						padding = New Integer(){paddingInt, paddingInt, paddingInt, paddingInt, paddingInt, paddingInt}
					End If
				End Try

			ElseIf dimension = 1 Then
				Dim paddingObj As Object = innerConfig(layerField)
				If TypeOf paddingObj Is System.Collections.IList Then
					Dim paddingList As IList(Of Integer) = DirectCast(paddingObj, System.Collections.IList)
					padding = New Integer(){ paddingList(0), paddingList(1) }
				Else
					Dim paddingInt As Integer = DirectCast(innerConfig(layerField), Integer)
					padding = New Integer(){paddingInt, paddingInt}
				End If

			Else
				Throw New UnsupportedKerasConfigurationException("Keras padding layer not supported")
			End If
			Return padding
		End Function
	End Class

End Namespace