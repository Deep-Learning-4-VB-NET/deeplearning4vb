Imports System
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
Imports TestAbstract = org.deeplearning4j.common.config.dummies.TestAbstract
Imports Test = org.junit.jupiter.api.Test
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.common.config

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Dl 4 J Class Loading Test") class DL4JClassLoadingTest
	Friend Class DL4JClassLoadingTest

		Private Const PACKAGE_PREFIX As String = "org.deeplearning4j.common.config.dummies."

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Create New Instance _ constructor Without Arguments") void testCreateNewInstance_constructorWithoutArguments()
		Friend Overridable Sub testCreateNewInstance_constructorWithoutArguments()
			' Given 
			Dim className As String = PACKAGE_PREFIX & "TestDummy"
			' When 
			Dim instance As Object = DL4JClassLoading.createNewInstance(className)
			' Then 
			assertNotNull(instance)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertEquals(className, instance.GetType().FullName)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Create New Instance _ constructor With Argument _ implicit Argument Types") void testCreateNewInstance_constructorWithArgument_implicitArgumentTypes()
		Friend Overridable Sub testCreateNewInstance_constructorWithArgument_implicitArgumentTypes()
			' Given 
			Dim className As String = PACKAGE_PREFIX & "TestColor"
			' When 
			Dim instance As TestAbstract = DL4JClassLoading.createNewInstance(className, New Object(){GetType(TestAbstract), "white"})
			' Then 
			assertNotNull(instance)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertEquals(className, instance.GetType().FullName)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Create New Instance _ constructor With Argument _ explicit Argument Types") void testCreateNewInstance_constructorWithArgument_explicitArgumentTypes()
		Friend Overridable Sub testCreateNewInstance_constructorWithArgument_explicitArgumentTypes()
			' Given 
			Dim colorClassName As String = PACKAGE_PREFIX & "TestColor"
			Dim rectangleClassName As String = PACKAGE_PREFIX & "TestRectangle"
			' When 
			Dim color As TestAbstract = DL4JClassLoading.createNewInstance(colorClassName, GetType(Object), New Type() { GetType(Integer), GetType(Integer), GetType(Integer) }, New Object(){45, 175, 200})
			Dim rectangle As TestAbstract = DL4JClassLoading.createNewInstance(rectangleClassName, GetType(Object), New Type() { GetType(Integer), GetType(Integer), GetType(TestAbstract) }, New Object(){10, 15, color})
			' Then 
			assertNotNull(color)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertEquals(colorClassName, color.GetType().FullName)
			assertNotNull(rectangle)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertEquals(rectangleClassName, rectangle.GetType().FullName)
		End Sub
	End Class

End Namespace