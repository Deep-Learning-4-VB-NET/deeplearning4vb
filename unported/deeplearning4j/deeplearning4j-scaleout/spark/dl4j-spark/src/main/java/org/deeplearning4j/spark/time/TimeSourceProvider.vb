Imports System
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties

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

Namespace org.deeplearning4j.spark.time


	Public Class TimeSourceProvider

		''' <summary>
		''' Default class to use when getting a TimeSource instance
		''' </summary>
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Public Shared ReadOnly DEFAULT_TIMESOURCE_CLASS_NAME As String = GetType(NTPTimeSource).FullName

		''' @deprecated Use <seealso cref="DL4JSystemProperties.TIMESOURCE_CLASSNAME_PROPERTY"/> 
		<Obsolete("Use <seealso cref=""DL4JSystemProperties.TIMESOURCE_CLASSNAME_PROPERTY""/>")>
		Public Const TIMESOURCE_CLASSNAME_PROPERTY As String = DL4JSystemProperties.TIMESOURCE_CLASSNAME_PROPERTY

		Private Sub New()
		End Sub

		''' <summary>
		''' Get a TimeSource
		''' the default TimeSource instance (default: <seealso cref="NTPTimeSource"/>
		''' </summary>
		''' <returns> TimeSource </returns>
		Public Shared ReadOnly Property Instance As TimeSource
			Get
				Dim className As String = System.getProperty(DL4JSystemProperties.TIMESOURCE_CLASSNAME_PROPERTY, DEFAULT_TIMESOURCE_CLASS_NAME)
    
				Return getInstance(className)
			End Get
		End Property

		''' <summary>
		''' Get a specific TimeSource by class name
		''' </summary>
		''' <param name="className"> Class name of the TimeSource to return the instance for </param>
		''' <returns> TimeSource instance </returns>
		Public Shared Function getInstance(ByVal className As String) As TimeSource
			Try
				Dim clazz As Type = DL4JClassLoading.loadClassByName(className)
'JAVA TO VB CONVERTER NOTE: The local variable getInstance was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim getInstance_Conflict As System.Reflection.MethodInfo = clazz.GetMethod("getInstance")
				Return DirectCast(getInstance_Conflict.invoke(Nothing), TimeSource)
			Catch e As Exception
				Throw New Exception("Error getting TimeSource instance for class """ & className & """", e)
			End Try
		End Function
	End Class

End Namespace