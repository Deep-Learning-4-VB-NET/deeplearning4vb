Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.deeplearning4j.common.config.DL4JSystemProperties.DISABLE_HELPER_PROPERTY
import static org.deeplearning4j.common.config.DL4JSystemProperties.HELPER_DISABLE_DEFAULT_VALUE

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
Namespace org.deeplearning4j.nn.layers

	''' <summary>
	''' Simple meta helper util class for instantiating
	''' platform specific layer helpers that handle interaction with
	''' lower level libraries like cudnn and onednn.
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class HelperUtils
	Public Class HelperUtils


		''' <summary>
		''' Creates a <seealso cref="LayerHelper"/>
		''' for use with platform specific code. </summary>
		''' @param <T> the actual class type to be returned </param>
		''' <param name="cudnnHelperClassName"> the cudnn class name </param>
		''' <param name="oneDnnClassName"> the one dnn class name </param>
		''' <param name="layerHelperSuperClass"> the layer helper super class </param>
		''' <param name="layerName"> the name of the layer to be created </param>
		''' <param name="arguments"> the arguments to be used in creation of the layer
		''' @return </param>
		Public Shared Function createHelper(Of T As LayerHelper)(ByVal cudnnHelperClassName As String, ByVal oneDnnClassName As String, ByVal layerHelperSuperClass As Type, ByVal layerName As String, ParamArray ByVal arguments() As Object) As T

			Dim disabled As Boolean? = Boolean.Parse(System.getProperty(DISABLE_HELPER_PROPERTY,HELPER_DISABLE_DEFAULT_VALUE))
			If disabled Then
				log.trace("Disabled helper creation, returning null")
				Return Nothing
			End If
			Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
			Dim helperRet As LayerHelper = Nothing
			If "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase) AndAlso cudnnHelperClassName IsNot Nothing AndAlso cudnnHelperClassName.Length > 0 Then
				If DL4JClassLoading.loadClassByName(cudnnHelperClassName) IsNot Nothing Then
					log.debug("Attempting to initialize cudnn helper {}",cudnnHelperClassName)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: helperRet = (LayerHelper) org.deeplearning4j.common.config.DL4JClassLoading.createNewInstance<LayerHelper>(cudnnHelperClassName, (@Class<? super LayerHelper>) layerHelperSuperClass, new Object[]{arguments});
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
					helperRet = DirectCast(DL4JClassLoading.createNewInstance(Of LayerHelper)(cudnnHelperClassName, CType(layerHelperSuperClass, Type(Of Object)), New Object(){arguments}), LayerHelper)
					log.debug("Cudnn helper {} successfully initialized",cudnnHelperClassName)

				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.warn("Unable to find class {}  using the classloader set for Dl4jClassLoading. Trying to use class loader that loaded the  class {} instead.",cudnnHelperClassName,layerHelperSuperClass.FullName)
					Dim classLoader As ClassLoader = DL4JClassLoading.Dl4jClassloader
					DL4JClassLoading.Dl4jClassloaderFromClass = layerHelperSuperClass
					Try
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: helperRet = (LayerHelper) org.deeplearning4j.common.config.DL4JClassLoading.createNewInstance<LayerHelper>(cudnnHelperClassName, (@Class<? super LayerHelper>) layerHelperSuperClass, arguments);
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
						helperRet = DirectCast(DL4JClassLoading.createNewInstance(Of LayerHelper)(cudnnHelperClassName, CType(layerHelperSuperClass, Type(Of Object)), arguments), LayerHelper)

					Catch e As Exception
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						log.warn("Unable to use  helper implementation {} for helper type {}, please check your classpath. Falling back to built in  normal  methods for now.",cudnnHelperClassName,layerHelperSuperClass.FullName)
					End Try

					log.warn("Returning class loader to original one.")
					DL4JClassLoading.Dl4jClassloader = classLoader

				End If

				If helperRet IsNot Nothing AndAlso Not helperRet.checkSupported() Then
					Return Nothing
				End If

				If helperRet IsNot Nothing Then
					log.debug("{} successfully initialized",cudnnHelperClassName)
				End If

			ElseIf "CPU".Equals(backend, StringComparison.OrdinalIgnoreCase) AndAlso oneDnnClassName IsNot Nothing AndAlso oneDnnClassName.Length > 0 Then
				helperRet = DL4JClassLoading.createNewInstance(Of LayerHelper)(oneDnnClassName, arguments)
				log.trace("Created oneDNN helper: {}, layer {}", oneDnnClassName,layerName)
			End If

			If helperRet IsNot Nothing AndAlso Not helperRet.checkSupported() Then
				log.debug("Removed helper {} as not supported", helperRet.GetType())
				Return Nothing
			End If

			Return DirectCast(helperRet, T)
		End Function

	End Class

End Namespace